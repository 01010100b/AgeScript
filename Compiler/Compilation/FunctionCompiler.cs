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

                    ExpressionCompiler.CompileExpression(script, function, rules, assign.Expression, address);
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        ExpressionCompiler.CompileExpression(script, function, rules, ret.Expression, script.CallResultBase);
                    }

                    rules.AddAction($"up-jump-direct g: {script.RegisterBase}");
                    rules.StartNewRule();
                    LastReturnStatement = i;
                }
                else if (statement is IfStatement ifs)
                {
                    ExpressionCompiler.CompileExpression(script, function, rules, ifs.Expression, script.CondGoal);
                    
                    var jmpid = Guid.NewGuid().ToString().Replace("-", "");
                    rules.StartNewRule($"goal {script.CondGoal} 0");
                    rules.AddAction($"up-jump-direct c: {jmpid}");
                    rules.StartNewRule();

                    i = CompileBlock(script, function, rules, i + 1);

                    rules.StartNewRule();
                    var ji = rules.CurrentRuleIndex;
                    rules.ReplaceStrings(jmpid, ji.ToString());
                }
                else if (statement is EndIfStatement)
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
