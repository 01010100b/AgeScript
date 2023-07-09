using AgeScript.Compiler.Intrinsics;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    internal class ExpressionCompiler
    {
        private Function Function { get; init; }

        public ExpressionCompiler(Function function)
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
                throw new NotSupportedException();
            }
        }

        private void CompileConst(CompilationResult result, ConstExpression expression,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            if (expression.Type == Primitives.Int)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:= {expression.Int}");
            }
            else if (expression.Type == Primitives.Bool)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:= {(expression.Bool ? 1 : 0)}");
            }
            else if (expression.Type == Primitives.Precise)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:= {expression.Precise}");
            }
            else
            {
                throw new NotSupportedException();
            }

            Utils.MemCopy(result, result.Memory.ConditionGoal, result_address.Value, expression.Type.Size, false, ref_result_address);
        }

        private void CompileAccessor(CompilationResult result, AccessorExpression expression,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            var ref_from_offset = false;
            int from_offset;

            if (expression.Accessor.Offset is ConstExpression c)
            {
                from_offset = c.Int;
            }
            else if (expression.Accessor.Offset is AccessorExpression acc)
            {
                if (acc.Accessor.Offset is not ConstExpression accc)
                {
                    throw new NotSupportedException();
                }

                from_offset = result.Memory.ExpressionGoal;
                ref_from_offset = true;

                Compile(result, expression.Accessor.Offset, from_offset);
                result.Rules.AddAction($"up-modify-goal {from_offset} c:* {((Array)expression.Accessor.Variable.Type).ElementType.Size}");
                result.Rules.AddAction($"up-modify-goal {from_offset} c:+ {accc.Int}");
            }
            else
            {
                throw new NotSupportedException();
            }

            var addr = result.Memory.GetAddress(expression.Accessor.Variable);
            Utils.MemCopy(result, addr, result_address.Value, expression.Type.Size,
                false, ref_result_address, from_offset, 0, ref_from_offset);
        }

        private void CompileCall(CompilationResult result, CallExpression expression,
            int? result_address, bool ref_result_address)
        {
            var called_function = result.Rules.GetFunction(expression.FunctionName, expression.Arguments, expression.Literal);
            
            if (result_address is not null)
            {
                if (expression.Type != called_function.ReturnType)
                {
                    throw new Exception("Call expression type does not match return type.");
                }
            }

            if (called_function is Inlined inlined)
            {
                inlined.CompileCall(result, expression, result_address, ref_result_address);

                return;
            }

            var register_count = result.Memory.GetRegisterCount(Function);
            var called_register_count = result.Memory.GetRegisterCount(called_function);

            // check for overflow

            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} g:= {result.Memory.StackPtr}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:+ {register_count}");
            result.Rules.StartNewRule($"up-compare-goal {result.Memory.ConditionGoal} c:> {result.Memory.StackLimit}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Error} c:= {(int)Errors.STACK_OVERFLOW}");
            result.Rules.AddAction($"up-jump-direct c: {result.Rules.EndTarget}");
            result.Rules.StartNewRule();

            // push registers to stack & clear registers & call result

            Utils.MemCopy(result, result.Memory.RegisterBase, result.Memory.StackPtr, register_count, false, true);
            Utils.Clear(result.Rules, result.Memory.RegisterBase, called_register_count);
            Utils.Clear(result.Rules, result.Memory.CallResultBase, called_function.ReturnType.Size);

            // set return address and parameters

            var return_target = result.Rules.CreateJumpTarget();
            result.Rules.AddAction($"up-modify-goal {result.Memory.RegisterBase} c:= {return_target}");

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

                        Utils.MemCopy(result, result.Memory.StackPtr, par_addr, par.Type.Size, true, false, offset);
                    }
                    else
                    {
                        CompileAccessor(result, acc, par_addr, false);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            // increment stack-ptr

            result.Rules.AddAction($"up-modify-goal {result.Memory.StackPtr} c:+ {register_count}");

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

            result.Rules.AddAction($"up-modify-goal {result.Memory.StackPtr} c:- {register_count}");
            Utils.MemCopy(result, result.Memory.StackPtr, result.Memory.RegisterBase, register_count, true);

            if (result_address is not null)
            {
                Utils.MemCopy(result, result.Memory.CallResultBase, result_address.Value, expression.Type.Size, false, ref_result_address);
            }
        }
    }
}
