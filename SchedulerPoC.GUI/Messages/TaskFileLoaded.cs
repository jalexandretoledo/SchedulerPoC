using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.GUI.Messages
{
    internal class TaskFileLoaded
    {
        public string Filename { get; }
        public int Count { get; }
        public TaskFileLoaded(string filename, int count)
        {
            Filename = filename;
            Count = count;
        }
    }
}
