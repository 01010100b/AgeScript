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
    internal class ObjectTypeCountTotalLessThan : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public ObjectTypeCountTotalLessThan() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "object_id", Type = Primitives.Int });
            Parameters.Add(new() { Name = "max", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"set-goal {script.Intr2} 0");

            rules.StartNewRule($"up-object-type-count-total g: {script.Intr0} g:< {script.Intr1}");
            rules.AddAction($"set-goal {script.Intr2} 1");

            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr2, result_address.Value, ReturnType.Size, false, ref_result_address);
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 0");

            result.Rules.StartNewRule($"up-object-type-count-total g: {result.Memory.Intr0} g:< {result.Memory.Intr1}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 1");

            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr2, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
