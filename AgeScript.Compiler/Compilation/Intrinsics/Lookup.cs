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
    internal class Lookup : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public Lookup() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "index", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            if (result_address is null)
            {
                return;
            }

            var table = script.Tables.Single(x => x.Name == cl.Literal.Replace("\"", ""));
            var ret_id = Script.GetUniqueId();

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-modify-goal {script.Intr1} g:= {script.Intr0}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:z/ {TableCompiler.Modulus}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:+ {table.AddressableName}");
            rules.AddAction($"set-goal {script.SpecialGoal} {ret_id}");
            rules.AddAction($"up-jump-direct g: {script.Intr1}");

            rules.StartNewRule();
            rules.ReplaceStrings(ret_id, rules.CurrentRuleIndex.ToString());
            rules.AddAction($"up-modify-goal {script.Intr1} g:= {script.Intr0}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:mod {TableCompiler.Modulus}");
            Utils.MemCopy(script, rules, script.TableResultBase, result_address.Value, Primitives.Int.Size, false, ref_result_address,
                script.Intr1, 0, true, false);
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            if (result_address is null)
            {
                return;
            }

            var table = result.Rules.GetTable(cl.Literal.Replace("\"", ""));
            var table_target = result.Rules.GetJumpTarget(table);
            var return_target = result.Rules.CreateJumpTarget();

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:z/ {result.Settings.TableModulus}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:+ {table_target}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} {return_target}");
            result.Rules.AddAction($"up-jump-direct g: {result.Memory.Intr1}");

            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(return_target);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:mod {result.Settings.TableModulus}");
            Utils.MemCopy2(result, result.Memory.TableResultBase, result_address.Value, 1, false, ref_result_address,
                result.Memory.Intr1, 0, true, false);
        }
    }
}
