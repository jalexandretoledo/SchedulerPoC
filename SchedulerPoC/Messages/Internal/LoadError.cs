using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Messages.Internal
{
    internal class LoadError
    {
        public Exception? Exception { get; }
        public String Description { get; }

        public LoadError(Exception ex)
        {
            Exception = ex;
            Description = ex.Message;
        }

        public LoadError(string desc) 
        {
            Description = desc;
        }


    }
}
