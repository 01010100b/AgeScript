using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal class GetObjectTypeCount : Intrinsic
    {
        public override bool HasStringLiteral => throw new NotImplementedException();

        public GetObjectTypeCount() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "object_type_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            const int MIN = 0;
            const int MAX = 1024;
            const int MID = (MAX + MIN) / 2;

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"set-goal {result.Memory.Intr1} {MIN}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} {MAX}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr3} {MID}");
            var end_target = result.Rules.CreateJumpTarget();

            result.Rules.StartNewRule($"up-compare-goal {result.Memory.Intr3} g:<= {result.Memory.Intr1}");
            var loop_target = result.Rules.CreateJumpTarget();
            result.Rules.ResolveJumpTarget(loop_target);
            result.Rules.AddAction($"up-jump-direct c: {end_target}");

            result.Rules.StartNewRule($"up-object-type-count g: {result.Memory.Intr0} g:< {result.Memory.Intr3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} g:= {result.Memory.Intr3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr3} g:+ {result.Memory.Intr1}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr3} c:z/ 2");
            result.Rules.AddAction($"up-jump-direct c: {loop_target}");

            result.Rules.StartNewRule($"up-object-type-count g: {result.Memory.Intr0} g:>= {result.Memory.Intr3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:= {result.Memory.Intr3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr3} g:+ {result.Memory.Intr2}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr3} c:z/ 2");
            result.Rules.AddAction($"up-jump-direct c: {loop_target}");

            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(end_target);
            Utils.MemCopy(result, result.Memory.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
