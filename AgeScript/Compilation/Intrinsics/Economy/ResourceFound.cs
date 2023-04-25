using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Economy
{
    internal class ResourceFound : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResourceFound() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "resource", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("resource must be const expression.");
            }

            rules.AddAction($"set-goal {script.Intr0} 0");
            rules.StartNewRule($"resource-found {ce.Int}");
            rules.AddAction($"set-goal {script.Intr0} 1");
            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
