using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public class ScheduledTasksList
    {
        public IReadOnlyList<ScheduledTaskStatus> ScheduledTasks { get; }
        public ScheduledTasksList(IEnumerable<ScheduledTaskStatus> tasks)
        {
            var result = new List<ScheduledTaskStatus>();
            result.AddRange(tasks);
            ScheduledTasks = result; 
        }

    }
}
