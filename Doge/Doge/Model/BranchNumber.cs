using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class BranchNumber
    {
        public int Ip { get; set; }
        public int Iq { get; set; }
        public int Np { get; set; }

        public BranchNumber(int ip, int iq, int np)
        {
            Ip = ip;
            Iq = iq;
            Np = np;
        }

        public override string ToString()
        {
            return $@"ip={Ip}&iq={Iq}&np={Np}";
        }
    }
}
