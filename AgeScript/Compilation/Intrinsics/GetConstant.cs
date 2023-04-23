using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetConstant : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public GetConstant()
        {
            Name = "GetConstant";
            ReturnType = Primitives.Int;
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            rules.AddAction($"set-goal {script.Intr0} {cl.Literal.Replace("\"", string.Empty)}");
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
