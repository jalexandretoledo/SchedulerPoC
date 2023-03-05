using Akka.Actor;
using Akka.Event;
using SchedulerPoC.Messages;
using SchedulerPoC.Messages.Internal;
using SchedulerPoC.Tasks;
using SchedulerPoC.Tasks.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Actors
{
    internal class TaskRunner
        : ReceiveActor
    {

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private ScheduledTask WorkingTask;
        private IActorRef coordinatorRef;

        public TaskRunner(IActorRef coordinator)
        {
            coordinatorRef = coordinator;
            Become(Idle);
        }


        public void Idle()
        {
            Receive<StartTask>(st =>
            {
                WorkingTask = st.ScheduledTask;
                RunTask();
                Become(Working);
            });

            ReceiveAny(msg =>
            {
                logger.Warning("Received {0} while in Idle mode, ignored.", msg);
            });
        }

        public void Working()
        {
            Receive<TaskRunError>(error =>
            {
                coordinatorRef.Tell(error);
                Become(Idle);
            });

            Receive<TaskRunCompleted>(done =>
            {
                coordinatorRef.Tell(done);
                Become(Idle);
            });

            ReceiveAny(msg =>
            {
                logger.Warning("Received {0} while in Working mode, ignored.", msg);
            });
        }

        private static ProcessStartInfo GetProcessStartInfo(SchedulerPoC.Tasks.Task task)
        {
            if (task is PythonTask pyTask)
            {
                // TODO: VirtualEnv

                var psi = new ProcessStartInfo()
                {
                    FileName = pyTask.PythonScript,
                    WorkingDirectory = Path.GetDirectoryName(pyTask.PythonScript),
                    UseShellExecute = false,
                };

                foreach (var p in pyTask.Parameters)
                {
                    psi.ArgumentList.Add(p);
                }

                return psi;
            }
            else if (task is ExcelTask xlTask)
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = SchedulerConfig.ExcelPath,
                    WorkingDirectory = Path.GetDirectoryName(xlTask.ExcelFile),
                };

                psi.ArgumentList.Add(xlTask.ExcelFile);

                return psi;
            }

            throw new TaskRunException($"Unknow task type: {task}");
        }

        private void RunTask() 
        {
            var self = Self;
            var task = WorkingTask;
            string output = "";
            int exitCode = 0;

            // TODO capture StandardError too?
            // TODO better error handling?
            System.Threading.Tasks.Task.Run(() =>
                {
                    var startInfo = GetProcessStartInfo(task.Task);

                    startInfo.RedirectStandardOutput = true;

                    var process = new Process()
                    {
                        StartInfo = startInfo,
                    };

                    process.Start();
                    output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                    exitCode = process.ExitCode;
                })
                .ContinueWith<Object>(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                        {
                            return new TaskRunError(task, exitCode, output);
                        }
                        else
                        {
                            return new TaskRunError(task, exitCode, output);
                        }
                    }
                    else if (x.IsCanceled)
                    {
                        return new TaskRunError(task, exitCode, output);
                    }

                    return new TaskRunCompleted(task, exitCode, output);
                })
                .PipeTo(self);
        }


    }
}
