using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.RastrPart
{
    public class CurrentLimit
    {
        public double Temperature { get; set; }
        public double LongTermCurrentLimit { get; set; }
        public double ShortTermCurrentLimit { get; set; }

        public override string ToString()
        {
            return $@"t={Temperature}°C ДДТН={LongTermCurrentLimit} АДТН={ShortTermCurrentLimit}";
        }
    }
}
