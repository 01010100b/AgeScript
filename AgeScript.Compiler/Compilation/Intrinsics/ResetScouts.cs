using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class ResetScouts : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResetScouts() : base()
        {
            ReturnType = Primitives.Void;
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            rules.AddAction("up-reset-scouts");
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            result.Rules.AddAction("up-reset-scouts");
        }
    }
}
