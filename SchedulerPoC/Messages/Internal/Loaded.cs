using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages.Internal
{
    internal class Loaded
    {
        public String Source { get; }
        public Int32 TasksLoaded { get; }

        public Loaded(string source, int tasksLoaded)
        {
            Source = source;
            TasksLoaded = tasksLoaded;
        }  
    }
}
