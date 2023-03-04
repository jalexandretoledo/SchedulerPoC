using SchedulerPoC.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages
{
    internal class TaskRunCompleted
    {
        public ScheduledTask ScheduledTask { get; }
        public int OutputCode { get; }
        public string? Output { get; }

        public TaskRunCompleted(ScheduledTask scheduledTask, int outputCode, string? output)
        {
            ScheduledTask = scheduledTask;
            OutputCode = outputCode;
            Output = output;
        }
    }
}
