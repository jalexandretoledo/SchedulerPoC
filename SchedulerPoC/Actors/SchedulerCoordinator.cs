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
        private readonly List<ScheduledTaskStatus> tasks = new List<ScheduledTaskStatus>();

        private readonly List<(bool IsBusy, IActorRef Runner)> runners = new List<(bool, IActorRef)>();

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
                Become(Working);
            });

            Receive<LoadError>(msg =>
            {

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
                var scheduled = tasks.FirstOrDefault(ts => ts.ScheduledTask == msg.ScheduledTask);
                if (scheduled != null)
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        if (RunTask(msg)) 
                        {
                            tasks.Remove(scheduled);
                            tasks.Add(new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Running));
                        }
                        else
                        {
                            logger.Warning("No runner available, will try again in 1 minute");
                            var key = $"{msg.ScheduledTask.Schedule:HH:mm} {msg.ScheduledTask.Task.Description} ({msg.ScheduledTask.Task.GetType()})";
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
                var scheduled = tasks.FirstOrDefault(ts => ts.ScheduledTask == msg.ScheduledTask);
                if (scheduled != null)
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        logger.Warning("Task isn't running yet: {0}", msg.ScheduledTask);
                    }
                    else if (scheduled.Status == ScheduleStatus.Running)
                    {
                        tasks.Remove(scheduled);
                        var completed = new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Completed);
                        tasks.Add(completed);
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
                var scheduled = tasks.FirstOrDefault(ts => ts.ScheduledTask == msg.ScheduledTask);
                if (scheduled != null)
                {
                    logger.Info("Task is already scheduled: {0}", msg.ScheduledTask);
                }
                else
                {
                    tasks.Add(new ScheduledTaskStatus(msg.ScheduledTask, ScheduleStatus.Waiting));
                }
            });

            Receive<RemoveScheduledTask>(msg =>
            {
                var scheduled = tasks.FirstOrDefault(ts => ts.ScheduledTask == msg.ScheduledTask);
                if (scheduled != null)
                {
                    if (scheduled.Status == ScheduleStatus.Waiting)
                    {
                        tasks.Remove(scheduled);
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
                Sender.Tell(new ScheduledTasksList(tasks));
            });

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
                                tasks.AddRange(task.ScheduledTasks);
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
            var pair = runners.FirstOrDefault(pair => !pair.IsBusy);
            var runner = pair.Runner;

            if (runner != null)
            {
                runner.Tell(task);
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
    }
}
