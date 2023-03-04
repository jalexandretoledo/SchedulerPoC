using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Tasks
{
    public sealed record ExcelTask
        : Task
    {
        public string ExcelFile { get; }

        public ExcelTask(string desc, string excelFile)
            : base(desc)
        {
            ExcelFile = excelFile;
        }
    }
}
