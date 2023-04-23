﻿using AgeScript.Compilation.Intrinsics;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using AgeScript.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    internal class FunctionCompiler
    {
        private ExpressionCompiler ExpressionCompiler { get; } = new();
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
                    var ref_address = false;

                    if (assign.Accessor is not null)
                    {
                        if (assign.Accessor.Offset is ConstExpression const_offset)
                        {
                            address = assign.Accessor.Variable.Address + const_offset.Int;
                            ref_address = false;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    ExpressionCompiler.Compile(script, function, rules, assign.Expression, address, ref_address);
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        ExpressionCompiler.Compile(script, function, rules, ret.Expression, script.CallResultBase);
                    }

                    rules.AddAction($"up-jump-direct g: {script.RegisterBase}");
                    rules.StartNewRule();
                    LastReturnStatement = i;
                }
                else if (statement is IfStatement ifs)
                {
                    ExpressionCompiler.Compile(script, function, rules, ifs.Expression, script.SpecialGoal);

                    var jmpid = Script.GetUniqueId();
                    rules.StartNewRule($"goal {script.SpecialGoal} 0");
                    rules.AddAction($"up-jump-direct c: {jmpid}");
                    rules.StartNewRule();

                    i = CompileBlock(script, function, rules, i + 1);

                    statement = function.Statements[i];

                    if (statement is ElifStatement)
                    {
                        var jmpidend = Script.GetUniqueId();

                        while (statement is ElifStatement elif)
                        {
                            rules.AddAction($"up-jump-direct c: {jmpidend}");
                            rules.StartNewRule();
                            var ji = rules.CurrentRuleIndex;
                            rules.ReplaceStrings(jmpid, ji.ToString());

                            ExpressionCompiler.Compile(script, function, rules, elif.Expression, script.SpecialGoal);

                            jmpid = Script.GetUniqueId();
                            rules.StartNewRule($"goal {script.SpecialGoal} 0");
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
                    }
                    else if (statement is EndIfStatement)
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
                    throw new NotSupportedException();
                }
            }

            return function.Statements.Count - 1;
        }
    }
}