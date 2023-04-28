using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class SetSn : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetSn() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "sn", Type = Primitives.Int });
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression sn)
            {
                throw new Exception("sn must be const expression.");
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-sn {sn.Int} g:= {result.Memory.Intr0}");
        }
    }
}
