using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Calculation
{
    public class Repair
    {
        public string Name { get; set; }
        public List<RastrElement> SwitchedElements { get; set; }
        public List<Accident> Accidents { get; set; }
        public double DeltaP { get; set; }

        public Repair()
        {
            SwitchedElements = new List<RastrElement>();
            Accidents = new List<Accident>();
        }

        public override string ToString()
        {
            return Name;
        }

        public double? GetPowerValueByStep(double step)
        {
            var firstAccident = Accidents.First();
            if ((step < firstAccident.StepValues.First()) || (step > firstAccident.StepValues.Last()))
                return null;
            for (int i = 0; i < firstAccident.StepValues.Count - 1; i++)
            {
                var previousStep = firstAccident.StepValues[i];
                var nextStep = firstAccident.StepValues[i+1];
                var previousPowerValue = firstAccident.PowerValues[i];
                var nextPowerValue = firstAccident.PowerValues[i + 1];
                if (step >= previousStep && step <= nextStep)
                {
                    var y1 = previousPowerValue;
                    var y2 = nextPowerValue;
                    var x1 = previousStep;
                    var x2 = nextStep;
                    var x = step;
                    return ((y2 - y1) * x + x2 * y1 - x1 * y2) / (x2 - x1);
                }
            }
            return null;
        }
    }
}
