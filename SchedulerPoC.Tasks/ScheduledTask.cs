using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public sealed record ScheduledTask
    {
        public Task Task { get; }
        public DateTime Schedule { get; }

        public ScheduledTask(Task task, DateTime schedule)
        {
            Task = task;
            Schedule = schedule;
        }
    }
}
