﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Parsing
{
    internal class ExpressionParser
    {
        public Expression Parse(Script script, Function function, string expression,
            IReadOnlyDictionary<string, string> literals)
        {
            if (expression.Contains('('))
            {
                var bo = expression.IndexOf('(');
                var bc = expression.LastIndexOf(')');
                var name = expression[..bo].Trim();
                var args = expression[(bo + 1)..bc].Trim().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                var arguments = new List<Expression>();
                string? literal = null;

                for (int i = 0; i < args.Count; i++)
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

                if (script.TryGetFunction(name, arguments, literal, out var f))
                {
                    var cex = new CallExpression()
                    {
                        Function = f!,
                        Arguments = arguments,
                        Literal = literal
                    };

                    cex.Validate();

                    return cex;
                }
                else
                {
                    throw new Exception("Can not find function.");
                }
            }
            else if (TryParseAccessor(script, function, expression, out var accessor))
            {
                var expr = new AccessorExpression() { Accessor = accessor! };
                expr.Validate();

                return expr;
            }
            else
            {
                var expr = new ConstExpression(expression);
                expr.Validate();

                return expr;
            }

            throw new Exception("Failed to parse expression.");
        }

        public bool TryParseAccessor(Script script, Function function, string code, out Accessor? accessor)
        {
            if (!code.Contains('['))
            {
                if (function.TryGetScopedVariable(script, code, out var variable))
                {
                    accessor = new()
                    {
                        Variable = variable!,
                        Offset = ConstExpression.Zero,
                        Type = variable!.Type
                    };

                    accessor.Validate();

                    return true;
                }
                else
                {
                    accessor = null;

                    return false;
                }
            }
            else
            {
                var pieces = code.Split('[');
                var vname = pieces[0].Trim();
                var offset = int.Parse(pieces[1].Replace("]", string.Empty).Trim());

                if (function.TryGetScopedVariable(script, vname, out var variable))
                {
                    if (variable!.Type is Array atype)
                    {
                        accessor = new()
                        {
                            Variable = variable!,
                            Offset = new ConstExpression((offset * atype.ElementType.Size).ToString()),
                            Type = atype.ElementType
                        };

                        accessor.Validate();

                        return true;
                    }
                    else
                    {
                        throw new Exception("Accessor must index an array type.");
                    }
                }
                else
                {
                    throw new Exception("Can not find variable for accessor.");
                }
            }
        }
    }
}
