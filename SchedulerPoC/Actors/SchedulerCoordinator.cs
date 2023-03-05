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

        private ActorSystem ActorSystem { get; }
        private readonly ILoggingAdapter logger = Context.GetLogger();
        private readonly Dictionary<string, ScheduledTaskStatus> tasks = new Dictionary<string, ScheduledTaskStatus>();

        private readonly List<Runner> runners = new List<Runner>();

        private readonly Dictionary<string, IActorRef> subscribers = new Dictionary<string, IActorRef>();

        public IStash? Stash { get; set; }
        public ITimerScheduler? Timers { get; set; }

        public SchedulerCoordinator(ActorSystem actorSys, ScheduledTasksList? newTasksList)
        {
            ActorSystem = actorSys;

            if (newTasksList != null)
            {
                foreach (var st in newTasksList.ScheduledTasks)
                {
                    tasks[st.ScheduledTask.TaskId] = st;
                }
            }

            for (var i = 0; i < SchedulerConfig.RunnersCount; i++)
            {
                var runnerRef = ActorSystem.ActorOf(Props.Create(() => new TaskRunner(Self)), $"runner_{i + 1}");
                runners.Add(new Runner(runnerRef));
            }

            Become(Loading);
            Self.Tell(new Loaded());
        }


        private void Loading()
        {
            Receive<Loaded>(msg =>
            {
                BecomeWorking();
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
                    var status = ScheduleStatus.Waiting;
                    if (msg.ScheduledTask.Schedule < DateTime.Now)
                    {
                        status = ScheduleStatus.Past;
                    }

                    var sts = new ScheduledTaskStatus(msg.ScheduledTask, status);
                    tasks[msg.ScheduledTask.TaskId] = sts;
                    ScheduleTrigger(sts);
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

            Receive<Subscribe>(msg =>
            {
                subscribers[msg.Id.ToString()] = Sender;
            });

            Receive<Subscribe>(msg =>
            {
                subscribers.Remove(msg.Id.ToString());
            });
        }

        private void BecomeWorking()
        {
            foreach (var task in tasks.Values)
            {
                ScheduleTrigger(task);
            }

            Become(Working);
        }

        private void ScheduleTrigger(ScheduledTaskStatus sts)
        {
            var status = sts.Status;
            if (status == ScheduleStatus.Waiting)
            {
                var runTime = sts.ScheduledTask.Schedule;

                var when = (runTime - DateTime.Now);
                if (when.TotalSeconds < 30)
                {
                    when = TimeSpan.FromSeconds(30);
                }

                Timers.StartSingleTimer(sts.ScheduledTask.TaskId, new StartTask(sts.ScheduledTask), when);
            }
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
            foreach (var subscriber in subscribers.Values)
            {
                subscriber.Tell(new ScheduledTasksList(tasks.Values));
            }
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
