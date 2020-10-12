using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Doge.Model.Calculation
{
    public class Accident
    {
        public string Name { get; set; }
        public List<RastrElement> SwitchedElements { get; set; }
        public List<double> PowerValues { get; set; }
        public List<double> StepValues { get; set; }
        public List<ObservableBranch> ObservableBranches { get; set; }

        public Accident()
        {
            SwitchedElements = new List<RastrElement>();
            ObservableBranches = new List<ObservableBranch>();
            PowerValues = new List<double>();
            StepValues = new List<double>();
        }

        public void AddValuesFromCurrentStep(RastrOperations rastr)
        {
            var powerValue = rastr.GetValue(@"sechen", @"psech", @"1");
            var stepvalue = rastr.GetValue("ut_common", @"sum_kfc", @"1");
            PowerValues.Add((double)powerValue);
            StepValues.Add((double)stepvalue);
            foreach (var observableBranch in ObservableBranches)
            {
                var currentValue = (double)rastr.GetValue("vetv", @"i_max", observableBranch.BranchElement.Selection) * 1000;
                observableBranch.CurrentValues.Add(currentValue);
            }
        }

        public Overload GetLongTermOverload(double temperature, List<BranchElementWithCurrentLimits> branchElementsWithCurrentLimits)
        {
            var overLoad = new Overload();
            overLoad.Step = double.MaxValue;
            foreach (var branchElementWithCurrentLimit in branchElementsWithCurrentLimits)
            {
                var longTermCurrentLimit = branchElementWithCurrentLimit.GetLongTimeCurrentLimitValue(temperature);
                var observableBranch = ObservableBranches.FirstOrDefault(k =>
                                                                        k.BranchElement.Ip == branchElementWithCurrentLimit.Ip &&
                                                                        k.BranchElement.Iq == branchElementWithCurrentLimit.Iq &&
                                                                        k.BranchElement.Np == branchElementWithCurrentLimit.Np);
                for (int i = 0; i < observableBranch.CurrentValues.Count-1; i++)
                {
                    var previousCurrentValue = observableBranch.CurrentValues[i];
                    var nextCurrentValue = observableBranch.CurrentValues[i+1];
                    var previousStep = StepValues[i];
                    var nextStep = StepValues[i+1];
                    if (longTermCurrentLimit >= previousCurrentValue && longTermCurrentLimit <= nextCurrentValue)
                    {
                        var y1 = previousStep;
                        var y2 = nextStep;
                        var x1 = previousCurrentValue;
                        var x2 = nextCurrentValue;
                        var x = longTermCurrentLimit;
                        var step = ((x-x1)*(y2-y1)/(x2-x1))+y1;
                        if (step < overLoad.Step)
                        {
                            overLoad.Step = step;
                            overLoad.CurrentLimitValue = longTermCurrentLimit;
                            overLoad.branchElementWithCurrentLimit = branchElementWithCurrentLimit;
                        }
                        break;
                    }
                }
            }
            if (overLoad.Step < double.MaxValue)
                return overLoad;
            else
                return null;
            
        }

        public Overload GetShortTermOverload(double temperature, List<BranchElementWithCurrentLimits> branchElementsWithCurrentLimits)
        {
            var overLoad = new Overload();
            overLoad.Step = double.MaxValue;
            foreach (var branchElementWithCurrentLimit in branchElementsWithCurrentLimits)
            {
                var shortTermCurrentLimit = branchElementWithCurrentLimit.GetShortTimeCurrentLimitValue(temperature);
                var observableBranch = ObservableBranches.FirstOrDefault(k =>
                                                                        k.BranchElement.Ip == branchElementWithCurrentLimit.Ip &&
                                                                        k.BranchElement.Iq == branchElementWithCurrentLimit.Iq &&
                                                                        k.BranchElement.Np == branchElementWithCurrentLimit.Np);
                for (int i = 0; i < observableBranch.CurrentValues.Count - 1; i++)
                {
                    var previousCurrentValue = observableBranch.CurrentValues[i];
                    var nextCurrentValue = observableBranch.CurrentValues[i + 1];
                    var previousStep = StepValues[i];
                    var nextStep = StepValues[i + 1];
                    if (shortTermCurrentLimit >= previousCurrentValue && shortTermCurrentLimit <= nextCurrentValue)
                    {
                        var y1 = previousStep;
                        var y2 = nextStep;
                        var x1 = previousCurrentValue;
                        var x2 = nextCurrentValue;
                        var x = shortTermCurrentLimit;
                        var step = ((x - x1) * (y2 - y1) / (x2 - x1)) + y1;
                        if (step < overLoad.Step)
                        {
                            overLoad.Step = step;
                            overLoad.CurrentLimitValue = shortTermCurrentLimit;
                            overLoad.branchElementWithCurrentLimit = branchElementWithCurrentLimit;
                        }
                        break;
                    }
                }
            }
            if (overLoad.Step < double.MaxValue)
                return overLoad;
            else
                return null;
        }

        public double GetStepForPowerValue(double powerValue)
        {

                for (int i = 0; i < PowerValues.Count - 1; i++)
                {
                
                    var previousPowerValue = PowerValues[i];
                    var nextPowerValue = PowerValues[i+1];
                    var previousStep = StepValues[i];
                    var nextStep = StepValues[i + 1];
                    if (powerValue >= previousPowerValue && powerValue <= nextPowerValue)
                    {
                        var y1 = previousStep;
                        var y2 = nextStep;
                        var x1 = previousPowerValue;
                        var x2 = nextPowerValue;
                        var x = powerValue;
                        var step = ((x - x1) * (y2 - y1) / (x2 - x1)) + y1;
                    return step;
                    }
                }
            return 0.0;
        }



        public override string ToString()
        {
            return Name;
        }
    }
}
