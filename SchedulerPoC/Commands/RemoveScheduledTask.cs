using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulerPoC.Messages;
using SchedulerPoC.Tasks;

namespace SchedulerPoC.Commands
{
    public class RemoveScheduledTask
    {
        public ScheduledTask ScheduledTask { get; }
        public RemoveScheduledTask(ScheduledTask scheduledTask)
        {
            ScheduledTask = scheduledTask;
        }
    }
}
