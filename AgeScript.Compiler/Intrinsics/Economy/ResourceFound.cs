using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Economy
{
    internal class ResourceFound : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ResourceFound() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "resource", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("resource must be const expression.");
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} c:= 0");
            result.Rules.StartNewRule($"resource-found {ce.Int}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} c:= 1");
            result.Rules.StartNewRule();
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
