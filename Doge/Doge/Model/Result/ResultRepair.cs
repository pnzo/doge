using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Result
{
    public class ResultRepair
    {
        public string Name { get; set; }
        public List<ResultTemperature> ResultTemperatures { get; set; }
        public double Plimit { get; set; }
        public double P8 { get; set; }
        public double P20 { get; set; }
        public ResultRepair()
        {
            ResultTemperatures = new List<ResultTemperature>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
