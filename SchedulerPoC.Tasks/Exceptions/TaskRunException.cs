using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks.Exceptions
{
    public class TaskRunException
        : Exception
    {

        public TaskRunException(string msg)
            : base(msg) 
        {
        }

        public TaskRunException(string msg, Exception ex)
            : base(msg, ex)
        {
        }

    }
}
