using AgeScript.Compiler.Intrinsics;
using AgeScript.Language.Expressions;
using AgeScript.Language.Statements;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    internal class FunctionCompiler
    {
        private int LastReturnStatement { get; set; } = -2;

        public void Compile(CompilationResult result, Function function)
        {
            if (function is Intrinsic)
            {
                return;
            }

            Console.WriteLine($"Compiling function {function.Name}");

            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(result.Rules.GetJumpTarget(function));
            LastReturnStatement = -2;

            var expression_compiler = new ExpressionCompiler(function);
            CompileBlock(result, function, 0, expression_compiler);

            if (LastReturnStatement < function.Statements.Count - 1)
            {
                // no return statement at end of function, add one in
                result.Rules.AddAction($"up-jump-direct g: {result.Memory.RegisterBase}");
            }

            result.Rules.StartNewRule();
        }

        private int CompileBlock(CompilationResult result, Function function, int index, ExpressionCompiler expression_compiler)
        {
            for (int i = index; i < function.Statements.Count; i++)
            {
                var statement = function.Statements[i];

                if (statement is ElifStatement || statement is EndIfStatement || statement is EndWhileStatement)
                {
                    return i;
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        expression_compiler.Compile(result, ret.Expression, result.Memory.CallResultBase);
                    }

                    result.Rules.AddAction($"up-jump-direct g: {result.Memory.RegisterBase}");
                    result.Rules.StartNewRule();
                    LastReturnStatement = i;
                }
                else if (statement is AssignStatement assign)
                {
                    int? address = null;
                    var ref_address = false;

                    if (assign.Accessor is not null)
                    {
                        if (assign.Accessor.Offset is ConstExpression const_offset)
                        {
                            address = result.Memory.GetAddress(assign.Accessor.Variable) + const_offset.Int;
                            ref_address = false;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    expression_compiler.Compile(result, assign.Expression, address, ref_address);
                }
                else if (statement is IfStatement ifs)
                {
                    expression_compiler.Compile(result, ifs.Condition, result.Memory.ConditionGoal);
                    var target_next = result.Rules.CreateJumpTarget();
                    result.Rules.StartNewRule($"goal {result.Memory.ConditionGoal} 0");
                    result.Rules.AddAction($"up-jump-direct c: {target_next}");
                    result.Rules.StartNewRule();

                    i = CompileBlock(result, function, i + 1, expression_compiler);
                    statement = function.Statements[i];

                    if (statement is ElifStatement)
                    {
                        var target_end = result.Rules.CreateJumpTarget();

                        while (statement is ElifStatement elif)
                        {
                            result.Rules.AddAction($"up-jump-direct c: {target_end}");
                            result.Rules.StartNewRule();
                            result.Rules.ResolveJumpTarget(target_next);
                            target_next = result.Rules.CreateJumpTarget();

                            expression_compiler.Compile(result, elif.Condition, result.Memory.ConditionGoal);
                            result.Rules.StartNewRule($"goal {result.Memory.ConditionGoal} 0");
                            result.Rules.AddAction($"up-jump-direct c: {target_end}");
                            result.Rules.StartNewRule();

                            i = CompileBlock(result, function, i + 1, expression_compiler);
                            statement = function.Statements[i];
                        }

                        if (statement is not EndIfStatement)
                        {
                            throw new Exception("Else/elif without endif.");
                        }

                        result.Rules.StartNewRule();
                        result.Rules.ResolveJumpTarget(target_next);
                        result.Rules.ResolveJumpTarget(target_end);
                    }
                    else if (statement is EndIfStatement)
                    {
                        result.Rules.StartNewRule();
                        result.Rules.ResolveJumpTarget(target_next);
                    }
                    else
                    {
                        throw new Exception("If without endif.");
                    }
                }
                else if (statement is WhileStatement ws)
                {
                    result.Rules.StartNewRule();
                    var target_return = result.Rules.CreateJumpTarget();
                    result.Rules.ResolveJumpTarget(target_return);

                    expression_compiler.Compile(result, ws.Condition, result.Memory.ConditionGoal);
                    var target_end = result.Rules.CreateJumpTarget();
                    result.Rules.StartNewRule($"goal {result.Memory.ConditionGoal} 0");
                    result.Rules.AddAction($"up-jump-direct c: {target_end}");
                    result.Rules.StartNewRule();

                    i = CompileBlock(result, function, i + 1, expression_compiler);
                    statement = function.Statements[i];

                    if (statement is EndWhileStatement)
                    {
                        result.Rules.AddAction($"up-jump-direct c: {target_return}");
                        result.Rules.StartNewRule();
                        result.Rules.ResolveJumpTarget(target_end);
                    }
                    else
                    {
                        throw new Exception("While without endwhile.");
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return function.Statements.Count - 1;
        }
    }
}
