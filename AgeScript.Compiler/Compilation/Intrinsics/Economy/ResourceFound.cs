using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Economy
{
    internal class ResourceFound : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResourceFound() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "resource", Type = Primitives.Int });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("resource must be const expression.");
            }

            result.Rules.AddAction($"set-goal {result.Memory.Intr0} 0");
            result.Rules.StartNewRule($"resource-found {ce.Int}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr0} 1");
            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
