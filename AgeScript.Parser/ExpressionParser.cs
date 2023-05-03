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
            Expression? expr = null;

            if (expression.Contains('('))
            {
                var bo = expression.IndexOf('(');
                var bc = expression.LastIndexOf(')');
                var name = expression[..bo].Trim();
                var args = expression[(bo + 1)..bc].SplitFull(",");
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
                        var argexpr = Parse(script, function, arg, literals, null);

                        if (argexpr is CallExpression)
                        {
                            throw new Exception("Can not nest call expressions.");
                        }

                        argexpr.Validate();
                        arguments.Add(argexpr);
                    }
                }

                if (type is not null)
                {
                    expr = new CallExpression()
                    {
                        FunctionName = name,
                        ReturnType = type,
                        Arguments = arguments,
                        Literal = literal
                    };
                }
                else
                {
                    throw new Exception("No type given for call expression.");
                }
            }
            else if (TryParseAccessor(script, function, expression, literals, out var accessor))
            {
                expr = new AccessorExpression() { Accessor = accessor! };
            }
            else if (expression.Contains('.'))
            {
                if (float.TryParse(expression, out var f))
                {
                    expr = ConstExpression.FromPrecise(f);
                }
                else
                {
                    var pieces = expression.SplitFull(".");

                    if (pieces.Count != 2)
                    {
                        throw new Exception("Failed to parse property.");
                    }

                    var table = script.Tables.SingleOrDefault(x => x.Name == pieces[0]);

                    if (table is not null)
                    {
                        if (pieces[1] == "Length")
                        {
                            expr = ConstExpression.FromInt(table.Length);
                        }
                        else
                        {
                            throw new Exception("Property not recognized.");
                        }
                    }

                    if (function.TryGetScopedVariable(script, pieces[0], out var variable))
                    {
                        if (pieces[1] == "Length")
                        {
                            if (variable!.Type is Array array)
                            {
                                expr = ConstExpression.FromInt(array.Length);
                            }
                            else
                            {
                                throw new Exception("Only array variables have Length property.");
                            }
                        }
                        else
                        {
                            throw new Exception("Property not recognized.");
                        }
                    }
                }
            }
            else if (int.TryParse(expression, out var i))
            {
                expr = ConstExpression.FromInt(i);
            }
            else if (bool.TryParse(expression, out var b))
            {
                expr = ConstExpression.FromBool(b);
            }
            else if (float.TryParse(expression, out var f))
            {
                expr = ConstExpression.FromPrecise(f);
            }

            if (expr is null)
            {
                throw new Exception("Failed to parse expression.");
            }

            expr.Validate();

            return expr;
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
                var pieces = code.SplitFull("[");

                if (pieces.Count != 2)
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
                            offset = ConstExpression.FromInt(oc.Int * atype.ElementType.Size);
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
