using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
   public class ObservableBranch
    {
        public string Name { get; set; }
        public BranchNumber BranchNumber { get; set; }
        public List<double> CurrentValues { get; set; }
    }
}
