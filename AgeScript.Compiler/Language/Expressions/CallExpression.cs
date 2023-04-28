using AgeScript.Compiler.Compilation.Intrinsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language.Expressions
{
    public class CallExpression : Expression
    {
        public required Function Function { get; init; }
        public required String FunctionName { get; init; }
        public required IReadOnlyList<Expression> Arguments { get; init; }
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

                if (a is CallExpression)
                {
                    throw new Exception("Can not nest call expressions.");
                }

                if (a.Type != p.Type)
                {
                    throw new Exception("Argument type mismatch");
                }
            }

            if (Literal is not null)
            {
                if (Function is Intrinsic intr)
                {
                    if (!intr.HasStringLiteral)
                    {
                        throw new Exception("Intrinsic has no string literal argument.");
                    }
                }
                else
                {
                    throw new Exception("Only intrinsics can have string literal arguments.");
                }
            }
        }
    }
}
