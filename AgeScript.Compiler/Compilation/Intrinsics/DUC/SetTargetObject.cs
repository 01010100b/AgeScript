using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics.DUC
{
    internal class SetTargetObject : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetTargetObject() : base()
        {
            Parameters.Add(new() { Name = "search_source", Type = Primitives.Int });
            Parameters.Add(new() { Name = "index", Type = Primitives.Int });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("search_source must be const expression.");
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr0);
            result.Rules.AddAction($"up-set-target-object {ce.Int} g: {result.Memory.Intr0}");
        }
    }
}
