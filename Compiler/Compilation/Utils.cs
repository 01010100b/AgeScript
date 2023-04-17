using Compiler.Compilation.Intrinsics;
using Compiler.Language.Expressions;
using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal static class Utils
    {
        public static void CompileExpression(Script script, Function function, RuleList rules,
            Expression expression, int? address)
        {
            if (expression is ConstExpression cst && address is not null)
            {
                if (cst.Type.Name == "Int")
                {
                    var value = int.Parse(cst.Value);
                    rules.AddAction($"set-goal {address} {value}");
                }
                else if (cst.Type.Name == "Bool")
                {
                    var value = bool.Parse(cst.Value);
                    var iv = value ? 1 : 0;
                    rules.AddAction($"set-goal {address} {iv}");
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (expression is VariableExpression vr && address is not null)
            {
                MemCopy(rules, vr.Variable.Address, address.Value, vr.Variable.Type.Size, false, false);
            }
            else if (expression is CallExpression cl)
            {
                if (cl.Function is Intrinsic intr)
                {
                    intr.Compile(rules, cl, address);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void MemCopy(RuleList rules, int from, int to, int length, bool ref_from, bool ref_to,
            int from_offset = 0, int to_offset = 0)
        {
            rules.AddAction($"set-goal sp0 {length}");

            if (ref_from)
            {
                rules.AddAction($"up-get-indirect-goal g: {from} sp1");
            }
            else
            {
                rules.AddAction($"set-goal sp1 {from}");
            }

            if (from_offset != 0)
            {
                rules.AddAction($"up-modify-goal sp1 c:+ {from_offset}");
            }

            if (ref_to)
            {
                rules.AddAction($"up-get-indirect-goal g: {to} sp2");
            }
            else
            {
                rules.AddAction($"set-goal sp2 {to}");
            }

            if (to_offset != 0)
            {
                rules.AddAction($"up-modify-goal sp2 c:+ {to_offset}");
            }

            rules.StartNewRule("up-compare-goal sp0 c:> 0");
            rules.AddAction("up-get-indirect-goal g: sp1 sp3");
            rules.AddAction("up-set-indirect-goal g: sp2 g: sp3");
            rules.AddAction("up-modify-goal sp1 c:+ 1");
            rules.AddAction("up-modify-goal sp2 c:+ 1");
            rules.AddAction("up-modify-goal sp0 c:- 1");
            rules.AddAction("up-jump-rule -1");
            rules.StartNewRule();
        }
    }
}
