using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerPoC.Commands
{
    public class Unsubscribe
    {
        public Guid Id { get; }
        public Unsubscribe(Guid id) => Id = id;
    }
}
