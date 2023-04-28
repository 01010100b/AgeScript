using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using AgeScript.Compiler.Compilation.Intrinsics;
using AgeScript.Compiler.Language.Expressions;
using AgeScript.Compiler.Language;

namespace AgeScript.Compiler.Compilation
{
    internal class ExpressionCompiler
    {
        public void Compile(Script script, Function function, RuleList rules,
            Expression expression, int? result_address = null, bool ref_result_address = false)
        {
            if (expression is AccessorExpression accessor)
            {
                CompileAccessor(script, function, rules, accessor, result_address, ref_result_address);
            }
            else if (expression is ConstExpression cst)
            {
                CompileConst(script, function, rules, cst, result_address, ref_result_address);
            }
            else
            {
                if (ref_result_address)
                {
                    throw new NotImplementedException();
                }

                CompileExpressionOld(script, function, rules, expression, result_address);
            }
        }

        private static void CompileAccessor(Script script, Function function, RuleList rules,
            AccessorExpression expression, int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            var from_offset = 0;
            var ref_from_offset = false;

            if (expression.Accessor.Offset is ConstExpression c)
            {
                from_offset = c.Int;
            }
            else
            {
                throw new NotImplementedException();
            }

            Utils.MemCopy(script, rules, expression.Accessor.Variable.Address, result_address.Value, expression.Accessor.Type.Size,
                false, ref_result_address, from_offset, 0, ref_from_offset);
        }

        private static void CompileConst(Script script, Function function, RuleList rules,
            ConstExpression expression, int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            if (expression.Type == Primitives.Int)
            {
                rules.AddAction($"set-goal {script.SpecialGoal} {expression.Int}");
            }
            else if (expression.Type == Primitives.Bool)
            {
                rules.AddAction($"set-goal {script.SpecialGoal} {(expression.Bool ? 1 : 0)}");
            }
            else if (expression.Type == Primitives.Precise)
            {
                rules.AddAction($"set-goal {script.SpecialGoal} {expression.Precise}");
            }
            else
            {
                throw new NotImplementedException();
            }

            Utils.MemCopy(script, rules, script.SpecialGoal, result_address.Value, 1, false, ref_result_address);
        }

        private static void CompileExpressionOld(Script script, Function function, RuleList rules,
            Expression expression, int? address)
        {
            if (expression is CallExpression cl)
            {
                CompileCallExpressionOld(script, function, rules, cl, address);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void CompileCallExpressionOld(Script script, Function function, RuleList rules,
            CallExpression expression, int? address)
        {
            if (expression.Function is Intrinsic intr)
            {
                intr.CompileCall(script, function, rules, expression, address);

                return;
            }

            // check for overflow

            rules.AddAction($"up-modify-goal {script.SpecialGoal} g:= {script.StackPtr}");
            rules.AddAction($"up-modify-goal {script.SpecialGoal} c:+ {function.RegisterCount}");
            rules.StartNewRule($"up-compare-goal {script.SpecialGoal} c:> {script.StackLimit}");
            rules.AddAction($"set-goal {script.Error} {(int)Errors.STACK_OVERFLOW}");
            rules.AddAction($"up-jump-direct c: {MemoryCompiler.JUMP_END}");
            rules.StartNewRule();

            // push registers to stack && clear registers

            Utils.MemCopy(script, rules, script.RegisterBase, script.StackPtr, function.RegisterCount, false, true);
            Utils.Clear(rules, script.RegisterBase, expression.Function.RegisterCount);

            // set return addr and parameters

            var return_addr_id = Script.GetUniqueId();
            rules.AddAction($"set-goal {script.RegisterBase} {return_addr_id}");

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                var par = expression.Function.Parameters[i];
                var arg = expression.Arguments[i];

                if (arg is ConstExpression ce)
                {
                    CompileConst(script, function, rules, ce, par.Address, false);
                }
                else if (arg is AccessorExpression acc)
                {
                    if (script.GlobalVariables.Values.Contains(acc.Accessor.Variable))
                    {
                        CompileAccessor(script, function, rules, acc, par.Address, false);
                    }
                    else
                    {
                        // local variables are now on stack, interpret addr as offset from stack-ptr

                        var offset = acc.Accessor.Variable.Address - script.RegisterBase;

                        if (acc.Accessor.Offset is ConstExpression c)
                        {
                            offset += c.Int;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        Utils.MemCopy(script, rules, script.StackPtr, par.Address, acc.Accessor.Type.Size,
                            true, false, offset, 0);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // increment stack-ptr

            rules.AddAction($"up-modify-goal {script.StackPtr} c:+ {function.RegisterCount}");

            if (ScriptCompiler.Settings.Debug)
            {
                rules.AddAction($"up-modify-goal {script.DebugMaxStackSpaceUsed} g:max {script.StackPtr}");
            }

            // jump

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
