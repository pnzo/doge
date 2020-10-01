using Doge.Model.Rastr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.InitialData
{
    public class TaskAccident
    {
        public int? Number { get; set; }
        public string Name { get; set; }
        public List<IRastrElement> SwitchedElements { get; set; }

        public TaskAccident()
        {
            SwitchedElements = new List<IRastrElement>();
        }
    }
}
