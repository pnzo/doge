using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Calculation
{
   public class Overload
    {
        public BranchElementWithCurrentLimits branchElementWithCurrentLimit { get; set; }
        public double CurrentLimitValue;
        public double Step;
    }
}
