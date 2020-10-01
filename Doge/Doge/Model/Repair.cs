﻿using Doge.Model.Rastr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model
{
    public class Repair
    {
        public string Name { get; set; }
        public List<IRastrElement> SwitchedElements { get; set; }
        public List<Accident> Accidents { get; set; }
        public double DeltaP { get; set; }
    }
}
