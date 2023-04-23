using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class Lookup : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public Lookup()
        {
            Name = "Lookup";
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "i", Type = Primitives.Int });
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

            var lookup = script.Tables.Single(x => x.Name == cl.Literal.Replace("\"", ""));
            var ret_id = Script.GetUniqueId();

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-modify-goal {script.Intr1} g:= {script.Intr0}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:z/ {TableCompiler.Modulus}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:+ {lookup.AddressableName}");
            rules.AddAction($"set-goal {script.SpecialGoal} {ret_id}");
            rules.AddAction($"up-jump-direct g: {script.Intr1}");

            rules.StartNewRule();
            rules.ReplaceStrings(ret_id, rules.CurrentRuleIndex.ToString());
            rules.AddAction($"up-modify-goal {script.Intr1} g:= {script.Intr0}");
            rules.AddAction($"up-modify-goal {script.Intr1} c:mod {TableCompiler.Modulus}");
            Utils.MemCopy(script, rules, script.TableResultBase, result_address.Value, Primitives.Int.Size, false, ref_result_address,
                script.Intr1, 0, true, false);
        }
    }
}
