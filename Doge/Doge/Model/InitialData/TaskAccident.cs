using Doge.Model.RastrPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.InitialData
{
    public class TaskAccident
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public List<RastrElement> SwitchedElements { get; set; }

        public TaskAccident()
        {
            SwitchedElements = new List<RastrElement>();
        }

        public bool IsContainsInRepair(TaskRepair repair)
        {
            foreach (var accidentSwitchedElement in SwitchedElements)
            {
                var isElementContainsInRepair = false;
                foreach (var repairSwitchedElement in repair.SwitchedElements)
                {
                    if ((accidentSwitchedElement.Selection == repairSwitchedElement.Selection) && (accidentSwitchedElement.State == repairSwitchedElement.State))
                    {
                        isElementContainsInRepair = true;
                        break;
                    }
                }
                if (!isElementContainsInRepair)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
