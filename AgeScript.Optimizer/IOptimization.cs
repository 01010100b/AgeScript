using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public interface IOptimization
    {
        int Priority { get; }
        void Optimize(List<Rule> rules);
    }
}
