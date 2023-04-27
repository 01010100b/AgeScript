using AgeScript.Compilation.Intrinsics;
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
        private Script Script { get; init; }
        private RuleList Rules { get; init; }
        private Memory Memory { get; init; }
        private Settings Settings { get; init; }
        private int LastReturnStatement { get; set; }

        public FunctionCompiler(Script script, RuleList rules, Memory memory, Settings settings)
        {
            Script = script;
            Rules = rules;
            Memory = memory;
            Settings = settings;
        }

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
                    ExpressionCompiler.Compile(script, function, rules, ifs.Condition, script.SpecialGoal);

                    var jmpidnext = Script.GetUniqueId();
                    rules.StartNewRule($"goal {script.SpecialGoal} 0");
                    rules.AddAction($"up-jump-direct c: {jmpidnext}");
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
                            var jinext = rules.CurrentRuleIndex;
                            rules.ReplaceStrings(jmpidnext, jinext.ToString());

                            ExpressionCompiler.Compile(script, function, rules, elif.Condition, script.SpecialGoal);

                            jmpidnext = Script.GetUniqueId();
                            rules.StartNewRule($"goal {script.SpecialGoal} 0");
                            rules.AddAction($"up-jump-direct c: {jmpidnext}");
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
                        rules.ReplaceStrings(jmpidnext, jiend.ToString());
                    }
                    else if (statement is EndIfStatement)
                    {
                        rules.StartNewRule();
                        var jiend = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpidnext, jiend.ToString());
                    }
                    else
                    {
                        throw new Exception("If without endif.");
                    }

                }
                else if (statement is WhileStatement ws)
                {
                    rules.StartNewRule();
                    var jiret = rules.CurrentRuleIndex;
                    ExpressionCompiler.Compile(script, function, rules, ws.Condition, script.SpecialGoal);

                    var jmpidend = Script.GetUniqueId();
                    rules.StartNewRule($"goal {script.SpecialGoal} 0");
                    rules.AddAction($"up-jump-direct c: {jmpidend}");
                    rules.StartNewRule();

                    i = CompileBlock(script, function, rules, i + 1);
                    statement = function.Statements[i];

                    if (statement is EndWhileStatement)
                    {
                        rules.AddAction($"up-jump-direct c: {jiret}");
                        rules.StartNewRule();
                        var jiend = rules.CurrentRuleIndex;
                        rules.ReplaceStrings(jmpidend , jiend.ToString());
                    }
                    else
                    {
                        throw new Exception("While without endwhile.");
                    }
                }
                else if (statement is ElifStatement || statement is EndIfStatement || statement is EndWhileStatement)
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
