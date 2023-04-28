using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetRandom : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetRandom() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "max", Type = Primitives.Int });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }
            var big_max = (int.MaxValue - 100000000) / 3;

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-get-precise-time 0 {result.Memory.Intr1}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:mod {result.Memory.Intr0}");

            for (int i = 0; i < 3; i++)
            {
                result.Rules.AddAction($"generate-random-number {big_max}");
                result.Rules.AddAction($"up-get-fact 33 0 {result.Memory.Intr2}");
                result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:+ {result.Memory.Intr2}");
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:mod {result.Memory.Intr0}");
            Utils.MemCopy2(result, result.Memory.Intr1, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}