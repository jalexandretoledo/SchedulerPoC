using SchedulerPoC.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages.Internal
{
    internal class StartTask
    {
        public ScheduledTask ScheduledTask { get; }
        public StartTask(ScheduledTask scheduledTask)
        {
            ScheduledTask = scheduledTask;
        }
    }
}
