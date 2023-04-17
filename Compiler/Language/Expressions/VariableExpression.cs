using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Expressions
{
    internal class VariableExpression : Expression
    {
        public required Variable Variable { get; init; }
        public override Type Type => Variable.Type;

        public override void Validate()
        {

        }
    }
}
