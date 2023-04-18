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
    internal static class ExpressionCompiler
    {
        public static void CompileExpression(Script script, Function function, RuleList rules,
            Expression expression, int? address)
        {
            if (expression is ConstExpression cst)
            {
                CompileConstExpression(rules, cst, address);
            }
            else if (expression is VariableExpression ve)
            {
                CompileVariableExpression(script, rules, ve, address);
            }
            else if (expression is CallExpression cl)
            {
                CompileCallExpression(script, function, rules, cl, address);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void CompileConstExpression(RuleList rules, ConstExpression expression, int? address)
        {
            if (address is null)
            {
                return;
            }

            if (expression.Type.Name == "Int")
            {
                var value = int.Parse(expression.Value);
                rules.AddAction($"set-goal {address} {value}");
            }
            else if (expression.Type.Name == "Bool")
            {
                var value = bool.Parse(expression.Value);
                var iv = value ? 1 : 0;
                rules.AddAction($"set-goal {address} {iv}");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void CompileVariableExpression(Script script, RuleList rules, VariableExpression expression, int? address)
        {
            if (address is null)
            {
                return;
            }

            Utils.MemCopy(script, rules, expression.Variable.Address, address.Value, expression.Variable.Type.Size);
        }

        private static void CompileCallExpression(Script script, Function function, RuleList rules,
            CallExpression expression, int? address)
        {
            if (expression.Function is Intrinsic intr)
            {
                intr.CompileCall(script, function, rules, expression, address);

                return;
            }

            // push registers to stack

            Utils.MemCopy(script, rules, script.RegisterBase, script.StackPtr, function.RegisterCount, false, true);

            // set return addr and parameters

            var return_addr_id = Guid.NewGuid().ToString().Replace("-", "");
            rules.AddAction($"set-goal {script.RegisterBase} {return_addr_id}");

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                var par = expression.Function.Parameters[i];
                var arg = expression.Arguments[i];

                if (arg is ConstExpression ce)
                {
                    CompileConstExpression(rules, ce, par.Address);
                }
                else if (arg is VariableExpression ve)
                {
                    if (script.GlobalVariables.ContainsKey(ve.Variable.Name))
                    {
                        CompileVariableExpression(script, rules, ve, par.Address);
                    }
                    else
                    {
                        // local variables are on stack, interpret addr as offset from stack-ptr

                        var offset = ve.Variable.Address - script.RegisterBase;
                        Utils.MemCopy(script, rules, script.StackPtr, par.Address, ve.Variable.Type.Size,
                            true, false, offset, 0);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // increment stack-ptr and jump

            rules.AddAction($"up-modify-goal {script.StackPtr} c:+ {function.RegisterCount}");
            rules.AddAction($"up-jump-direct c: {expression.Function.AddressableName}");
            rules.StartNewRule();

            var return_addr = rules.CurrentRuleIndex;
            rules.ReplaceStrings(return_addr_id, return_addr.ToString());

            // pop registers from stack

            rules.AddAction($"up-modify-goal {script.StackPtr} c:- {function.RegisterCount}");
            Utils.MemCopy(script, rules, script.StackPtr, script.RegisterBase, function.RegisterCount, true);

            if (address is not null)
            {
                Utils.MemCopy(script, rules, script.CallResultBase, address.Value, expression.Type.Size);
            }
        }
    }
}
