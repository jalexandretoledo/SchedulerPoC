using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public sealed record ScheduledTask
    {
        public string TaskId { get; }
        public Task Task { get; }
        public DateTime Schedule { get; }

        public ScheduledTask(string taskId, Task task, DateTime schedule)
        {
            TaskId = taskId;
            Task = task;
            Schedule = schedule;
        }
    }
}
