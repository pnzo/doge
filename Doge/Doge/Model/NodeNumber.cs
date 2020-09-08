using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class NodeNumber
    {
        public int Ny;

        public NodeNumber(int ny)
        {
            Ny = ny;
        }

        public override string ToString()
        {
            return $@"ny={Ny}";
        }
    }
}
