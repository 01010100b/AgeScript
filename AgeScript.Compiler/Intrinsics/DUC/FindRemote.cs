using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class FindRemote : Inlined
    {
        public override bool HasStringLiteral => false;

        public FindRemote() : base()
        {
            ReturnType = Primitives.Int4;
            Parameters.Add(new() { Name = "player", Type =  Primitives.Int });
            Parameters.Add(new() { Name = "unit_id", Type = Primitives.Int });
            Parameters.Add(new() { Name = "max", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            ExpressionCompiler.Compile(result, cl.Arguments[2], result.Memory.Intr2);
            result.Rules.AddAction($"up-modify-sn 251 g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-find-remote g: {result.Memory.Intr1} g: {result.Memory.Intr2}");

            if (result_address is not null)
            {
                result.Rules.AddAction($"up-get-search-state {result.Memory.Intr0}");
                Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 4, false, ref_result_address);
            }
        }
    }
}
