using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Rastr

{
    public class NodeElement : IRastrElement
    {
        public int Ny { get; set; }
        public string Selection
        {
            get
            {
                return $@"ny={Ny}";
            }
            set { }
        }
        public bool State { get; set; }


        public NodeElement(int ny)
        {
            Ny = ny;
            State = false;
        }

        public override string ToString()
        {
            return Selection;
        }


    }
}
