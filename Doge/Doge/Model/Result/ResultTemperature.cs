using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Result
{
    public class ResultTemperature
    {
        public double TemperatureValue { get; set; }
        public double? Pddtn { get; set; }
        public double? DDTN { get; set; }
        public string CriteriaOfPddtn { get; set; }
        public double? Padtn { get; set; }
        public double? ADTN { get; set; }
        public string CriteriaOfPadtn { get; set; }
        public double MDP { get 
            {
                var minPadtn = double.MaxValue;
                var minP8 = double.MaxValue;
                foreach (var resultAccident in ResultAccidents)
                {
                    if ((resultAccident.Padtn != null) && (resultAccident.Padtn < minPadtn))
                        minPadtn = resultAccident.Padtn ?? 0;
                    if (resultAccident.P8BeforeAccident < minP8)
                        minP8 = resultAccident.P8BeforeAccident;
                }
                return Math.Min(minPadtn, minP8);
            } 
        }

        public override string ToString()
        {
            return $@"{TemperatureValue}";
        }

        public List<ResultAccident> ResultAccidents { get; set; }

        public ResultTemperature()
        {
            ResultAccidents = new List<ResultAccident>();
        }


    }
}
