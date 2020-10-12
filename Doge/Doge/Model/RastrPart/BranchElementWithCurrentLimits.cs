using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.RastrPart
{
    public class BranchElementWithCurrentLimits : RastrElement
    {
        public string Name { get; set; }
        public List<CurrentLimit> CurrentLimits { get; set; }
        public BranchElementWithCurrentLimits(int ip, int iq, int np) : base(ip, iq, np)
        {
            CurrentLimits = new List<CurrentLimit>();
        }

        public double GetShortTimeCurrentLimitValue(double temperature)
        {
            var currentLimits = CurrentLimits.OrderBy(k => k.Temperature).ToList(); ;
            if (temperature <= currentLimits.First().Temperature)
                return currentLimits.First().ShortTermCurrentLimit;
            if (temperature >= currentLimits.Last().Temperature)
                return currentLimits.Last().ShortTermCurrentLimit;
            for (int i = 0; i < currentLimits.Count-1; i++)
            {
                var previousLimit = currentLimits[i];
                var nextLimit = currentLimits[i+1];
                if (temperature >= previousLimit.Temperature && temperature <= nextLimit.Temperature)
                {
                    var y1 = previousLimit.ShortTermCurrentLimit;
                    var y2 = nextLimit.ShortTermCurrentLimit;
                    var x1 = previousLimit.Temperature;
                    var x2 = nextLimit.Temperature;
                    var x = temperature;
                    return ((y2 - y1) * x + x2 * y1 - x1 * y2) / (x2 - x1);
                }
            }
            return 0;
        }

        public double GetLongTimeCurrentLimitValue(double temperature)
        {
            var currentLimits = CurrentLimits.OrderBy(k => k.Temperature).ToList(); ;
            if (temperature <= currentLimits.First().Temperature)
                return currentLimits.First().LongTermCurrentLimit;
            if (temperature >= currentLimits.Last().Temperature)
                return currentLimits.Last().LongTermCurrentLimit;
            for (int i = 0; i < currentLimits.Count - 1; i++)
            {
                var previousLimit = currentLimits[i];
                var nextLimit = currentLimits[i + 1];
                if (temperature >= previousLimit.Temperature && temperature <= nextLimit.Temperature)
                {
                    var y1 = previousLimit.LongTermCurrentLimit;
                    var y2 = nextLimit.LongTermCurrentLimit;
                    var x1 = previousLimit.Temperature;
                    var x2 = nextLimit.Temperature;
                    var x = temperature;
                    return ((y2 - y1) * x + x2 * y1 - x1 * y2) / (x2 - x1);
                }
            }
            return 0;
        }

    }
}
