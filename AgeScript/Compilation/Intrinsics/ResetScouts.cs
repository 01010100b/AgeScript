using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class ResetScouts : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResetScouts()
        {
            Name = "ResetScouts";
            ReturnType = Primitives.Void;
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            rules.AddAction("up-reset-scouts");
        }
    }
}
