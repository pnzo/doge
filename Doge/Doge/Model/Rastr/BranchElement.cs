using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Rastr
{
    public class BranchElement : IRastrElement
    {
        public int Ip { get; set; }
        public int Iq { get; set; }
        public int Np { get; set; }
        public string Selection
        {
            get
            {
                return $@"ip={Ip}&iq={Iq}&np={Np}";
            }
            set { }
        }
        public bool State { get; set; }

        public BranchElement(int ip, int iq, int np)
        {
            Ip = ip;
            Iq = iq;
            Np = np;
            State = false;
        }

        public override string ToString()
        {
            return Selection;
        }
    }
}
