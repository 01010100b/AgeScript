using AgeScript.Compiler.Compilation.Intrinsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language.Expressions
{
    public class CallExpression : Expression
    {
        public required string FunctionName { get; init; }
        public required Type ReturnType { get; init; }
        public required IReadOnlyList<Expression> Arguments { get; init; }
        public required string? Literal { get; init; }
        public override Type Type => ReturnType;

        public override void Validate()
        {
            Named.ValidateName(FunctionName);
            ReturnType.Validate();

            foreach (var arg in Arguments)
            {
                arg.Validate();
            }
        }
    }
}
