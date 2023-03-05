using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.GUI.Messages
{
    internal class LoadFromFile
    {
        public string Filename { get; }
        public LoadFromFile(string filename)
        {
            Filename = filename;
        }   
    }
}
