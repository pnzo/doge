using Doge.Model.Rastr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.InitialData
{
    public class TaskRepair
    {
        public string Name { get; set; }
        public string OptionsString { get; set; }
        public List<IRastrElement> SwitchedElements { get; set; }

        public TaskRepair()
        {
            SwitchedElements = new List<IRastrElement>();
        }
    }
}
