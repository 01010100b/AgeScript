using Compiler.Compilation.Intrinsics;
using Compiler.Language;
using Compiler.Language.Expressions;
using Compiler.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class FunctionCompiler
    {
        private int LastReturnStatement { get; set; }

        public void Compile(Script script, Function function, RuleList rules)
        {
            if (function is Intrinsic)
            {
                return;
            }

            LastReturnStatement = -2;
            Console.WriteLine($"Compiling function {function.Name}");
            rules.StartNewRule();
            function.Address = rules.CurrentRuleIndex;
            CompileBlock(script, function, rules, 0);

            if (LastReturnStatement < function.Statements.Count - 1)
            {
                // no return statement at end of function, add one in

                rules.AddAction($"up-jump-direct g: {script.RegisterBase}");
            }

            rules.StartNewRule();
        }

        private int CompileBlock(Script script, Function function, RuleList rules, int index)
        {
            for (int i = index; i < function.Statements.Count; i++)
            {
                var statement = function.Statements[i];

                if (statement is AssignStatement assign)
                {
                    int? address = null;

                    if (assign.Variable is not null)
                    {
                        address = assign.Variable.Address + assign.Offset;
                    }

                    ExpressionCompiler.CompileExpressionOld(script, function, rules, assign.Expression, address);
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        ExpressionCompiler.CompileExpressionOld(script, function, rules, ret.Expression, script.CallResultBase);
                    }

                    rules.AddAction($"up-jump-direct g: {script.RegisterBase}");
                    rules.StartNewRule();
                    LastReturnStatement = i;
                }
                else if (statement is IfStatement ifs)
                {
                    ExpressionCompiler.CompileExpressionOld(script, function, rules, ifs.Expression, script.CondGoal);
                    
                    var jmpid = Guid.NewGuid().ToString().Replace("-", "");
                    rules.StartNewRule($"goal {script.CondGoal} 0");
                    rules.AddAction($"up-jump-direct c: {jmpid}");
                    rules.StartNewRule();

                    i = CompileBlock(script, function, rules, i + 1);

                    statement = function.Statements[i];

                    if (statement is ElifStatement)
                    {
                        var jmpidend = Guid.NewGuid().ToString().Replace("-", "");

                        while (statement is ElifStatement elif)
                        {
                            rules.AddAction($"up-jump-direct c: {jmpidend}");
                            rules.StartNewRule();
                            var ji = rules.CurrentRuleIndex;
                            rules.ReplaceStrings(jmpid, ji.ToString());

                            ExpressionCompiler.CompileExpressionOld(script, function, rules, elif.Expression, script.CondGoal);

                            jmpid = Guid.NewGuid().ToString().Replace("-", "");
                            rules.StartNewRule($"goal {script.CondGoal} 0");
                            rules.AddAction($"up-jump-direct c: {jmpid}");
                            rules.StartNewRule();

                            i = CompileBlock(script, function, rules, i + 1);

                            statement = function.Statements[i];
                        }

                        if (statement is not EndIfStatement)
                        {
                            throw new Exception("Else/elif without endif.");
                        }

                        rules.StartNewRule();
                        var jiend = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpidend, jiend.ToString());
                        rules.ReplaceStrings(jmpid, jiend.ToString());
                    }/*
                    else if (statement is ElseStatement els)
                    {
                        var jmpid2 = Guid.NewGuid().ToString().Replace("-", "");
                        rules.AddAction($"up-jump-direct c: {jmpid2}");
                        rules.StartNewRule();
                        var ji = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpid, ji.ToString());

                        i = CompileBlock(script, function, rules, i + 1);

                        statement = function.Statements[i];

                        if (statement is not EndIfStatement)
                        {
                            throw new Exception("Else without endif.");
                        }

                        rules.StartNewRule();
                        var ji2 = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpid2, ji2.ToString());
                    }*/
                    else if (statement is EndIfStatement eifs)
                    {
                        rules.StartNewRule();
                        var ji = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpid, ji.ToString());
                    }
                    else
                    {
                        throw new Exception("If without endif.");
                    }
                    
                }
                else if (statement is ElifStatement || statement is EndIfStatement)
                {
                    return i;
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
