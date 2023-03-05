using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulerPoC.Messages;
using SchedulerPoC.Tasks;

namespace SchedulerPoC.Commands
{
    public class AddScheduledTask
    {
        public ScheduledTask ScheduledTask { get; }
        public AddScheduledTask(ScheduledTask scheduledTask)
        {
            ScheduledTask = scheduledTask;
        }
    }
}
