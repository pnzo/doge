using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class Accident
    {
        public string Name;
        public List<BranchNumber> OpenedBranchesNumbers;
        public List<BranchNumber> ClosedBranchesNumbers;
        public List<NodeNumber> OpenedNodesNumbers;
        public List<NodeNumber> ClosedNodesNumbers;
        public List<double> PowerValue;
        public List<double> StepValue;
        public List<ObservableBranch> ObservableBranches;

    }
}
