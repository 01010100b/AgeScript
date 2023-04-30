using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Intrinsics
{
    internal class Lookup : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public Lookup() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "index", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            if (result_address is null)
            {
                return;
            }

            var table = result.Rules.GetTable(cl.Literal.Replace("\"", string.Empty));
            var table_target = result.Rules.GetJumpTarget(table);
            var return_target = result.Rules.CreateJumpTarget();

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:z/ {result.Settings.TableModulus}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:+ {table_target}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} c:= {return_target}");
            result.Rules.AddAction($"up-jump-direct g: {result.Memory.Intr1}");

            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(return_target);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:mod {result.Settings.TableModulus}");
            Utils.MemCopy(result, result.Memory.TableResultBase, result_address.Value, 1, false, ref_result_address,
                result.Memory.Intr1, 0, true, false);
        }
    }
}
