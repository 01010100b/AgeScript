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
        public void Compile(Script script, Function function, RuleList rules)
        {
            Console.WriteLine($"Compiling function {function.Name}");
            rules.StartNewRule();
            function.Address = rules.CurrentRuleIndex;
            CompileBlock(script, function, rules, 0);
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

                    CompileExpression(script, function, rules, assign.Expression, address);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return index;
        }

        private void CompileExpression(Script script, Function function, RuleList rules, 
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
                Utils.MemCopy(rules, vr.Variable.Address, address.Value, vr.Variable.Type.Size, false, false);
            }
            else if (expression is CallExpression cl)
            {
                if (cl.Function is Intrinsic intr)
                {
                    intr.Compile(rules, cl);
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
    }
}
