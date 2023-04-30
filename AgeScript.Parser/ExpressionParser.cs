using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Parser
{
    internal class ExpressionParser
    {
        public Expression Parse(Script script, Function function, string expression,
            IReadOnlyDictionary<string, string> literals, Type? type)
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
                        var expr = Parse(script, function, arg, literals, null);

                        if (expr is CallExpression)
                        {
                            throw new NotImplementedException();
                        }

                        expr.Validate();
                        arguments.Add(expr);
                    }
                }

                if (type is not null)
                {
                    var cex = new CallExpression()
                    {
                        FunctionName = name,
                        ReturnType = type,
                        Arguments = arguments,
                        Literal = literal
                    };

                    cex.Validate();

                    return cex;
                }
                else
                {
                    throw new Exception("No type given for call expression.");
                }
            }
            else if (TryParseAccessor(script, function, expression, literals, out var accessor))
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

        public bool TryParseAccessor(Script script, Function function, string code,
            IReadOnlyDictionary<string, string> literals, out Accessor? accessor)
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

                if (pieces.Length != 2)
                {
                    throw new Exception("Accessor not parsed.");
                }

                var vname = pieces[0].Trim();

                if (function.TryGetScopedVariable(script, vname, out var variable))
                {
                    if (variable!.Type is Array atype)
                    {
                        var oname = pieces[1].Replace("]", string.Empty).Trim();
                        var offset = Parse(script, function, oname, literals, Primitives.Int);

                        if (offset.Type != Primitives.Int)
                        {
                            throw new Exception("Offset must have type Int.");
                        }

                        if (offset is ConstExpression oc)
                        {
                            offset = new ConstExpression((oc.Int * atype.ElementType.Size).ToString());
                        }

                        accessor = new()
                        {
                            Variable = variable!,
                            Offset = offset,
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
