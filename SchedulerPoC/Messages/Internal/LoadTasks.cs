using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages.Internal
{
    internal class LoadTasks
    {
        public String Source { get; }

        public LoadTasks(string source)
        {
            Source = source;
        }
    }
}
