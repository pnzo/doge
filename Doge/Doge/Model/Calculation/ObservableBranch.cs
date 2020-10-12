using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Calculation
{
   public class ObservableBranch
    {
        public string Name { get; set; }
        public RastrElement BranchElement { get; set; }
        public List<double> CurrentValues { get; set; }

        public ObservableBranch(int ip, int iq, int np, string name)
        {
            BranchElement = new RastrElement(ip,iq,np);
            Name = name;
            CurrentValues = new List<double>();
        }

        public ObservableBranch()
        {

        }
        public override string ToString()
        {
            return Name;
        }
    }
}
