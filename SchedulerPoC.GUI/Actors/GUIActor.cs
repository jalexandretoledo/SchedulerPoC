using Akka.Actor;
using Akka.Event;
using SchedulerPoC.Commands;
using SchedulerPoC.GUI.Messages;
using SchedulerPoC.Tasks;
using SchedulerPoC.Tasks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.GUI.Actors
{
    internal class GUIActor
        : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();
        private IActorRef coordinator;
        private List<GridRowWrapper> taskList { get; }
        private DataGridView Grid { get; }

        public GUIActor(IActorRef coord, DataGridView grid)
        {
            coordinator = coord;

            taskList = new List<GridRowWrapper>();
            Grid = grid;

            Become(Running);
        }

        public void Running()
        {
            Receive<GetScheduledTasks>(msg =>
            {
                coordinator.Tell(msg);
            });

            Receive<ScheduledTasksList>(data =>
            {
                taskList.Clear();
                foreach (var task in data.ScheduledTasks)
                {
                    // TODO send AddTask message
                    taskList.Add(new GridRowWrapper(task));
                }

                Grid.DataSource = taskList;
                Grid.Refresh();
            });

            Receive<LoadFromFile>(msg =>
            {
                LoadTasksFromFile(msg.Filename);
            });

            Receive<LoadError>(error =>
            {
                logger.Error(error.Description, error.Exception);
            });

            Receive<TaskFileLoaded>(msg =>
            {
                Self.Tell(new GetScheduledTasks());
            });

            Receive<AddScheduledTask>(msg =>
            {
                coordinator.Tell(msg);
            });

            Receive<RemoveScheduledTask>(msg =>
            {
                coordinator.Tell(msg);
            });

            ReceiveAny(msg => logger.Info("Received {0}", msg));
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
                                foreach (var st in task.ScheduledTasks)
                                {
                                    self.Tell(new AddScheduledTask(st.ScheduledTask));
                                }
                                self.Tell(new TaskFileLoaded(source, task.ScheduledTasks.Count));
                            });
                })
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                        {
                            self.Tell(new LoadError(x.Exception));
                        }
                        else
                        {
                            self.Tell(new LoadError("Load was cancelled with an error"));
                        }
                    }
                    else if (x.IsCanceled)
                    {
                        self.Tell(new LoadError("Load was cancelled"));
                    }
                });
        }


        private class GridRowWrapper
        {
            public ScheduledTaskStatus Task { get; }
            private string _time { get; }

            public GridRowWrapper(ScheduledTaskStatus st)
            {
                Task = st;
                _time = Task.ScheduledTask.Schedule.ToString("HH:mm");
            }

            public string TaskId => Task.ScheduledTask.TaskId;
            public string Time => _time;
            public string Description => Task.ScheduledTask.Task.Description;
            public string Status => Task.Status.ToString();
        }

    }
}
