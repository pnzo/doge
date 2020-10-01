using Doge.Model.Rastr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class Accident
    {
        public string Name { get; set; }
        public List<IRastrElement> SwitchedElements { get; set; }
        public List<double> PowerValue { get; set; }
        public List<double> StepValue { get; set; }
        public List<ObservableBranch> ObservableBranches { get; set; }

    }
}
