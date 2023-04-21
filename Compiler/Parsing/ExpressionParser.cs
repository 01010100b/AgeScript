using Compiler.Language;
using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Parsing
{
    internal class ExpressionParser
    {
        public Expression Parse(Script script, Function function, string expression, 
            IReadOnlyDictionary<string, string> literals)
        {
            if (expression.Contains('('))
            {
                // call expression

                var bo = expression.IndexOf('(');
                var bc = expression.LastIndexOf(')');
                var name = expression[..bo].Trim();
                var args = expression[(bo + 1)..bc].Trim().Split(',');
                var arguments = new List<Expression>();
                string? literal = null;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].Trim();

                    if (literals.ContainsKey(arg))
                    {
                        if (i == 0)
                        {
                            literal = literals[arg];
                        }
                        else
                        {
                            throw new Exception("String literals can only be first argument.");
                        }
                    }
                    else
                    {
                        var expr = Parse(script, function, arg, literals);

                        if (expr is CallExpression)
                        {
                            throw new Exception("Can not nest call expressions.");
                        }

                        expr.Validate();

                        arguments.Add(expr);
                    }
                }

                var f = script.Functions.Single(x =>
                {
                    if (x.Name != name || x.Parameters.Count != arguments.Count)
                    {
                        return false;
                    }
                    else
                    {
                        for (int i = 0; i < arguments.Count; i++)
                        {
                            var a = arguments[i];
                            var p = x.Parameters[i];

                            if (a.Type != p.Type)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                });

                var cex = new CallExpression()
                {
                    Function = f,
                    Arguments = arguments,
                    Literal = literal
                };

                cex.Validate();

                return cex;
            }
            else if (function.TryGetScopedVariable(script, expression, out var variable))
            {
                // variable expression

                var expr = new VariableExpression() { Variable = variable!, Offset = 0, ElementType = variable!.Type };
                expr.Validate();

                return expr;
            }
            else
            {
                // const expression

                var expr = new ConstExpression(expression);
                expr.Validate();

                return expr;
            }

            throw new NotImplementedException();
        }
    }
}
