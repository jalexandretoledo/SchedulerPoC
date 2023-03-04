using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks.Exceptions
{
    public class TaskLoadException
        : Exception
    {

        public TaskLoadException(string description, Exception? innerException)
            : base(description, innerException)
        {
        }
    }
}
