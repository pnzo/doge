using Doge.Model.RastrPart;
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
        public List<RastrElement> SwitchedElements { get; set; }
        public List<string> ExcludeAccidentsLabels
        {
            get
            {
                var excludeAccidentsLabels = new List<string>();
                var parts = (OptionsString.Split(' ')).Where(k => k.StartsWith(@"#"));
                foreach (var part in parts)
                {
                    excludeAccidentsLabels.Add(part.TrimStart('#'));
                }
                return excludeAccidentsLabels;
            }
        }

        public List<string> IncludeAccidentsLabels
        {
            get
            {
                var includeAccidentsLabels = new List<string>();
                var parts = (OptionsString.Split(' ')).Where(k => k.StartsWith(@"$"));
                foreach (var part in parts)
                {
                    includeAccidentsLabels.Add(part);
                }
                return includeAccidentsLabels;
            }
        }

        public double? deltaP
        {
            get
            {
                var parts = OptionsString.Split(' ').Where(k=>k.StartsWith(@"p")).ToList();
                if (parts.Count() == 0)
                    return null;
                return double.Parse(parts.FirstOrDefault().TrimStart('p'));
            }
        }

        public TaskRepair()
        {
            SwitchedElements = new List<RastrElement>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
