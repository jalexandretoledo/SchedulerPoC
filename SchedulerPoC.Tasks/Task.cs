using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public abstract record Task
    {
        public string Description { get; }

        protected Task(string desc)
        {
            Description = desc;
        }

    }
}
