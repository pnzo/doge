using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class Repair
    {
        public string Name { get; set; }
        public List<BranchNumber> OpenedBranchesNumbers { get; set; }
        public List<BranchNumber> ClosedBranchesNumbers { get; set; }
        public List<NodeNumber> OpenedNodesNumbers { get; set; }
        public List<NodeNumber> ClosedNodesNumbers { get; set; }
        public List<double> PowerValue { get; set; }
        public List<double> StepValue { get; set; }
    }
}
