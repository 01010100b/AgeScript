using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Expressions
{
    internal abstract class Expression : Validated
    {
        public abstract Type Type { get; }
    }
}
