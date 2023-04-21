using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Parsing
{
    internal class ExpressionParser
    {
        private Dictionary<string, string> Literals { get; init; }

        public ExpressionParser(Dictionary<string, string> literals)
        {
            Literals = literals;
        }
    }
}
