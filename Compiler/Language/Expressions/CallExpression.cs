using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Expressions
{
    internal class CallExpression : Expression
    {
        public required Function Function { get; init; }
        public required List<Expression> Arguments { get; init; }
        public required string? Literal { get; init; }
        public override Type Type => Function.ReturnType;

        public override void Validate()
        {
            if (Function.Parameters.Count != Arguments.Count)
            {
                throw new Exception("Arguments must have same length as function parameters");
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                var a = Arguments[i];
                var p = Function.Parameters[i];

                a.Validate();

                if (a.Type != p.Type)
                {
                    throw new Exception("Argument type mismatch");
                }

                if (a is CallExpression)
                {
                    throw new Exception("Can not nest call expressions.");
                }
            }
        }
    }
}
