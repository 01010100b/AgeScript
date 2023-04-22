using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetPlayerNumber : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPlayerNumber() 
        {
            Name = "GetPlayerNumber";
            ReturnType = Primitives.Int;
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            rules.AddAction($"up-get-fact player-number 0 {script.Intr0}");
            Utils.MemCopy(script,rules, script.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
