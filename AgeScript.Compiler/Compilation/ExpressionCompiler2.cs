using AgeScript.Compiler.Compilation.Intrinsics;
using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation
{
    internal class ExpressionCompiler2
    {
        private Function Function { get; init; }

        public ExpressionCompiler2(Function function)
        {
            Function = function;
        }

        public void Compile(CompilationResult result, Expression expression,
            int? result_address = null, bool ref_result_address = false)
        {
            if (expression is ConstExpression ce)
            {
                CompileConst(result, ce, result_address, ref_result_address);
            }
            else if (expression is AccessorExpression acc)
            {
                CompileAccessor(result, acc, result_address, ref_result_address);
            }
            else if (expression is CallExpression cl)
            {
                CompileCall(result, cl, result_address, ref_result_address);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void CompileConst(CompilationResult result, ConstExpression expression,
            int? result_address,  bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            if (expression.Type == Primitives.Int)
            {
                result.Rules.AddAction($"set-goal {result.Memory.ConditionGoal} {expression.Int}");
            }
            else if (expression.Type == Primitives.Bool)
            {
                result.Rules.AddAction($"set-goal {result.Memory.ConditionGoal} {(expression.Bool ? 1 : 0)}");
            }
            else if (expression.Type == Primitives.Precise)
            {
                result.Rules.AddAction($"set-goal {result.Memory.ConditionGoal} {expression.Precise}");
            }
            else
            {
                throw new NotImplementedException();
            }

            Utils.MemCopy2(result, result.Memory.ConditionGoal, result_address.Value, expression.Type.Size, false, ref_result_address);
        }

        private void CompileAccessor(CompilationResult result, AccessorExpression expression,
            int? result_address, bool ref_result_address)
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

            var addr = result.Memory.GetAddress(expression.Accessor.Variable);
            Utils.MemCopy2(result, addr, result_address.Value, expression.Type.Size,
                false, ref_result_address, from_offset, 0, ref_from_offset);
        }

        private void CompileCall(CompilationResult result, CallExpression expression,
            int? result_address, bool ref_result_address)
        {
            var called_function = result.Rules.GetFunction(expression.FunctionName, expression.Arguments, expression.Literal);

            if (called_function is Intrinsic intrinsic)
            {
                intrinsic.CompileCall2(result, expression, result_address, ref_result_address);

                return;
            }

            // check for overflow

            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} g:= {result.Memory.StackPtr}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:+ {Function.RegisterCount}");
            result.Rules.StartNewRule($"up-compare-goal {result.Memory.ConditionGoal} c:> {result.Memory.StackLimit}");
            result.Rules.AddAction($"set-goal {result.Memory.Error} {(int)Errors.STACK_OVERFLOW}");
            result.Rules.AddAction($"up-jump-direct c: {result.Rules.EndTarget}");
            result.Rules.StartNewRule();

            // push registers to stack & clear registers

            Utils.MemCopy2(result, result.Memory.RegisterBase, result.Memory.StackPtr, Function.RegisterCount, false, true);
            Utils.Clear2(result, result.Memory.RegisterBase, called_function.RegisterCount);

            // set return address and parameters

            var return_target = result.Rules.CreateJumpTarget();
            result.Rules.AddAction($"set-goal {result.Memory.RegisterBase} {return_target}");

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                var par = called_function.Parameters[i];
                var arg = expression.Arguments[i];
                var par_addr = result.Memory.GetAddress(par);

                if (arg is ConstExpression ce)
                {
                    CompileConst(result, ce, par_addr, false);
                }
                else if (arg is AccessorExpression acc)
                {
                    if (Function.AllVariables.Contains(acc.Accessor.Variable))
                    {
                        // local variables are now on stack, interpret addr as offset from stack-ptr

                        var offset = result.Memory.GetAddress(acc.Accessor.Variable) - result.Memory.RegisterBase;

                        if (acc.Accessor.Offset is ConstExpression c)
                        {
                            offset += c.Int;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        Utils.MemCopy2(result, result.Memory.StackPtr, par_addr, par.Type.Size, true, false, offset);
                    }
                    else
                    {
                        CompileAccessor(result, acc, par_addr, false);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // increment stack-ptr

            result.Rules.AddAction($"up-modify-goal {result.Memory.StackPtr} c:+ {Function.RegisterCount}");

            if (result.Settings.Debug)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.DebugMaxStackSpaceUsed} g:max {result.Memory.StackPtr}");
            }

            // jump

            var jump_target = result.Rules.GetJumpTarget(called_function);
            result.Rules.AddAction($"up-jump-direct c: {jump_target}");
            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(return_target);

            // pop registers from stack

            result.Rules.AddAction($"up-modify-goal {result.Memory.StackPtr} c:- {Function.RegisterCount}");
            Utils.MemCopy2(result, result.Memory.StackPtr, result.Memory.RegisterBase, Function.RegisterCount, true);

            if (result_address is not null)
            {
                Utils.MemCopy2(result, result.Memory.CallResultBase, result_address.Value, expression.Type.Size, false, ref_result_address);
            }
        }
    }
}
