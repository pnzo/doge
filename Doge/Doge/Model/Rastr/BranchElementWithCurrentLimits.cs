using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Rastr
{
    public class BranchElementWithCurrentLimits : BranchElement
    {
        public List<CurrentLimit> CurrentLimits { get; set; }
        public BranchElementWithCurrentLimits(int ip, int iq, int np) : base(ip, iq, np)
        {
        }
    }
}
