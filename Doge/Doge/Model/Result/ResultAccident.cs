using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Result
{
   public class ResultAccident
    {
        public string Name { get; set; }
        public double? Padtn { get; set; }
        public string CriteriaOfPadtn { get; set; }
        public double? ADTN { get; set; }
        public double Plimit { get; set; }
        public double P8 { get; set; }
        public double P8BeforeAccident { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
