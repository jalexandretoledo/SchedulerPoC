using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public sealed record PythonTask
        : Task
    {
        public string PythonScript { get; }
        public string[] Parameters { get; }
        public string VirtualEnv { get; }

        public PythonTask(string desc, string pythonScript, string[] parameters, string virtualEnv)
            : base(desc)
        {
            PythonScript = pythonScript;
            Parameters = parameters;
            VirtualEnv = virtualEnv;
        }
    }
}
