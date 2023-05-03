using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class ResetSearch : Inlined
    {
        public override bool HasStringLiteral => false;

        public ResetSearch() : base()
        {
            ReturnType = Primitives.Int4;
            Parameters.Add(new() { Name = "local_index", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "local_list", Type= Primitives.Bool });
            Parameters.Add(new() { Name = "remote_index", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "remote_list", Type = Primitives.Bool });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression c0)
            {
                throw new Exception("local_index must be const.");
            }

            if (cl.Arguments[1] is not ConstExpression c1)
            {
                throw new Exception("local_list must be const.");
            }

            if (cl.Arguments[2] is not ConstExpression c2)
            {
                throw new Exception("remote_index must be const.");
            }

            if (cl.Arguments[3]  is not ConstExpression c3)
            {
                throw new Exception("remote_list must be const.");
            }

            result.Rules.AddAction($"up-reset-search {(c0.Bool ? 1 : 0)} {(c1.Bool ? 1 : 0)} {(c2.Bool ? 1 : 0)} {(c3.Bool ? 1 : 0)}");

            if (result_address is not null)
            {
                result.Rules.AddAction($"up-get-search-state {result.Memory.Intr0}");
                Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 4, false, ref_result_address);
            }
        }
    }
}
