using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public class ScheduledTaskStatus
    {
        public ScheduledTask ScheduledTask { get; }
        public ScheduleStatus Status { get; }

        public ScheduledTaskStatus(ScheduledTask scheduledTask, ScheduleStatus status)
        {
            ScheduledTask = scheduledTask;
            Status = status;
        }
    }
}
