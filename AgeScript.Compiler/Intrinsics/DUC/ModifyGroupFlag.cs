using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class ModifyGroupFlag : Inlined
    {
        public override bool HasStringLiteral => false;

        public ModifyGroupFlag() : base()
        {
            Parameters.Add(new() { Name = "remove", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "group", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression ce0)
            {
                throw new Exception("remove must be const");
            }

            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-group-flag {(ce0.Bool ? 0 : 1)} g: {result.Memory.Intr0}");
        }
    }
}
