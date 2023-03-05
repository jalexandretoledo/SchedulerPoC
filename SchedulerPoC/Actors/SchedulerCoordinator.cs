using Akka.Actor;
using Akka.Event;
using SchedulerPoC.Commands;
using SchedulerPoC.Messages;
using SchedulerPoC.Messages.Internal;
using SchedulerPoC.Tasks;
using SchedulerPoC.Tasks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Actors
{
    internal class SchedulerCoordinator
        : ReceiveActor,
        IWithUnboundedStash,
        IWithTimers
    {

        private readonly ILoggingAdapter logger = Context.GetLogger();
        private readonly Dictionary<string, ScheduledTaskStatus> tasks = new Dictionary<string, ScheduledTaskStatus>();

        private readonly List<Runner> runners = new List<Runner>();

        public IStash? Stash { get; set; }
        public ITimerScheduler? Timers { get; set; }

        public SchedulerCoordinator(String taskFile)
        {
            Become(Unloaded);
            Self.Tell(new LoadTasks(taskFile));
        }

        public void Unloaded() 
        {
            Receive<LoadTasks>(lt =>
            {
                LoadTasksFromFile(lt.Source);
                Become(Loading);
            });

            ReceiveAny(msg =>
            {
                logger.Warning("Received {0} while unloaded. Stashed.", msg);
                Stash.Stash();
            });
        }

        private void Loading()
        {
            Receive<Loaded>(msg =>
            {
                BecomeWorking();
            });

            Receive<LoadError>(msg =>
            {
                throw new TaskLoadException(msg.Description, msg.Exception);
            });

            ReceiveAny(msg =>
            {
                logger.Warning("Received {0} while loading. Stashed.", msg);
                Stash.Stash();
            });
        }

        private void Working()
        {
            Receive<StartTask>(msg =>
            {
                if (tasks.TryGetValue(msg.ScheduledTask.TaskId, out var scheduled))
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        if (RunTask(msg)) 
                        {
                            tasks[msg.ScheduledTask.TaskId] = new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Running);
                        }
                        else
                        {
                            logger.Warning("No runner available, will try again in 1 minute");
                            var key = MakeKey(msg.ScheduledTask);
                            Timers.StartSingleTimer(key, msg, TimeSpan.FromMinutes(1));
                        }
                    }
                    else if (scheduled.Status == ScheduleStatus.Running)
                    {
                        logger.Info("Task is running already: {0}", msg.ScheduledTask);
                    }
                    else
                    {
                        logger.Info("Task is completed already: {0}", msg.ScheduledTask);
                    }
                }
                else
                {
                    logger.Info("Task is not scheduled anymore: {0}", msg.ScheduledTask);
                }
            });

            Receive<TaskRunCompleted>(msg =>
            {
                if (tasks.TryGetValue(msg.ScheduledTask.TaskId, out var scheduled))
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        logger.Warning("Task isn't running yet: {0}", msg.ScheduledTask);
                    }
                    else if (scheduled.Status == ScheduleStatus.Running)
                    {
                        var completed = new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Completed);
                        tasks[msg.ScheduledTask.TaskId] = completed;
                        NotifyCompletion(completed);
                    }
                    else
                    {
                        logger.Info("Task is completed already: {0}", msg.ScheduledTask);
                    }
                }
                else
                {
                    logger.Info("Task is not scheduled anymore: {0}", msg.ScheduledTask);
                }

            });

            Receive<AddScheduledTask>(msg =>
            {
                if (tasks.TryGetValue(msg.ScheduledTask.TaskId, out var scheduled))
                {
                    logger.Info("Task is already scheduled: {0}", msg.ScheduledTask);
                }
                else
                {
                    tasks[msg.ScheduledTask.TaskId] = new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Waiting);
                }
            });

            Receive<RemoveScheduledTask>(msg =>
            {
                if (tasks.TryGetValue(msg.ScheduledTask.TaskId, out var scheduled))
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        tasks.Remove(scheduled.ScheduledTask.TaskId);
                    }
                    else if (scheduled.Status == ScheduleStatus.Running)
                    {
                        logger.Info("Task is running already, can't remove: {0}", msg.ScheduledTask);
                    }
                    else
                    {
                        logger.Info("Task is completed already, can't remove: {0}", msg.ScheduledTask);
                    }
                }
                else
                {
                    logger.Info("Task is not scheduled: {0}", msg.ScheduledTask);
                }
            });

            Receive<GetScheduledTasks>(msg =>
            {
                Sender.Tell(new ScheduledTasksList(tasks.Values));
            });

        }

        private void BecomeWorking()
        {
            foreach (var task in tasks.Values)
            {
                var status = task.Status;
                if (status == ScheduleStatus.Waiting)
                {
                    var runTime = task.ScheduledTask.Schedule;

                    var when = (runTime - DateTime.Now);
                    if (when.TotalSeconds < 30)
                    {
                        when = TimeSpan.FromSeconds(30);
                    }

                    Timers.StartSingleTimer(task.ScheduledTask.TaskId, new StartTask(task.ScheduledTask), when);
                }
            }

            Become(Working);
        }

        private void LoadTasksFromFile(string source)
        {
            var self = Self;
            System.Threading.Tasks.Task.Run(() =>
                {
                    TasksUtils.LoadTasksFromFile(source)
                        .Match(
                            error => throw new TaskLoadException(error, null),
                            task =>
                            {
                                tasks.Clear();
                                foreach (var st in task.ScheduledTasks)
                                {
                                    tasks[st.ScheduledTask.TaskId] = st;
                                }
                            });
                })
                .ContinueWith<Object>(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                        {
                            return new LoadError(x.Exception);
                        }
                        else
                        {
                            return new LoadError("Load was cancelled with an error");
                        }
                    }
                    else if (x.IsCanceled)
                    {
                        return new LoadError("Load was cancelled");
                    }

                    return new Loaded(source, tasks.Count);
                })
                .PipeTo(self);
        }

        private bool RunTask(StartTask task)
        {
            var runner = runners.FirstOrDefault(r => !r.IsBusy);
            if (runner != null)
            {
                runner.Start(task);
                return true;
            }
            else
            {
                logger.Info("No runner available");
                return false;
            }
        }

        private void NotifyCompletion(ScheduledTaskStatus scheduledTaskStatus)
        {

        }

        private static string MakeKey(ScheduledTask st)
        {
            return $"{st.Schedule:HH:mm} {st.Task.Description} ({st.Task.GetType()})";
        }

        private class Runner
        {
            public bool IsBusy { get; private set; }
            public IActorRef TaskRunner { get; set; }
            public Runner(IActorRef taskRunner)
            {
                TaskRunner = taskRunner;
                IsBusy = false;
            }

            public void Start(StartTask command)
            {
                if (IsBusy)
                {
                    throw new TaskRunException("Runner is already busy!");
                }

                IsBusy = true;
                TaskRunner.Tell(command);
            }

        }

    }
}
