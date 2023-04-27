using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript
{
    public class Settings
    {
        public int MaxGoal { get; set; } = 512;
        public int MaxElementsPerRule { get; set; } = 16;
        public bool OptimizeMemCopy { get; set; } = true; // 291-1666 -> 43-432
        public bool InlineMemCopy { get; set; } = true;
        public bool OptimizeComparisons { get; set; } = true;
        public bool Debug { get; set; } = false;
        public int TableModulus => MaxElementsPerRule - 2;
    }
}