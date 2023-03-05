using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Commands
{
    public class Subscribe
    {
        public Guid Id { get; }
        public Subscribe(Guid id) => Id = id;
    }
}
