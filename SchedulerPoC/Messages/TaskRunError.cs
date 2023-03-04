using SchedulerPoC.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages
{
    internal class TaskRunError
    {
        public ScheduledTask ScheduledTask { get; }
        public int OutputCode { get; }
        public string? Output { get; }

        public TaskRunError(ScheduledTask scheduledTask, int outputCode, string? output)
        {
            ScheduledTask = scheduledTask;
            OutputCode = outputCode;
            Output = output;
        }
    }
}
