using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.RastrPart
{
    public class RastrElement
    {
        public int Ip { get; set; }
        public int Iq { get; set; }
        public int Np { get; set; }
        public int Ny { get; set; }
        public string Selection
        {
            get
            {
                if (Ny != 0)
                    return $@"ny={Ny}";
                else
                    return $@"ip={Ip}&iq={Iq}&np={Np}";

            }
            set { }
        }

        public bool IsBranch
        {
            get
            {
                return Ip != 0;
            }
            set { }
        }
        public bool State { get; set; }

        public RastrElement(int ip, int iq, int np)
        {
            Ip = ip;
            Iq = iq;
            Np = np;
            State = false;
        }

        public RastrElement(int ny)
        {
            Ny = ny;
            State = false;
        }

        public RastrElement()
        {
        }

        public override string ToString()
        {
            return Selection;
        }
    }
}
