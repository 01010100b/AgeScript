using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal abstract class Intrinsic : Function
    {
        public abstract bool HasStringLiteral { get; }

        protected ExpressionCompiler ExpressionCompiler { get; } = new();

        internal abstract void CompileCall(Script script, Function function, RuleList rules,
            CallExpression cl, int? result_address, bool ref_result_address = false);
    }
}
