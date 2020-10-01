using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Rastr
{
    public interface IRastrElement
    {
        string Selection { get; set; }
        bool State { get; set; }
    }
}
