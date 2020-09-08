using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class BranchNumber
    {
        public int Ip;
        public int Iq;
        public int Np;

        public BranchNumber(int ip,int iq, int np)
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
