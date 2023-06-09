﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Parser
{
    internal class StatementParser
    {
        private ExpressionParser ExpressionParser { get; } = new();

        public Statement Parse(Script script, Function function, string line,
            IReadOnlyDictionary<string, string> literals)
        {
            if (line == "return" || line.StartsWith("return "))
            {
                var expr = line[6..].Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    if (function.ReturnType != Primitives.Void)
                    {
                        throw new Exception("return without value.");
                    }

                    return new ReturnStatement() { Expression = null };
                }
                else
                {
                    if (function.ReturnType == Primitives.Void)
                    {
                        throw new Exception("Can not return anything from a function with return type Void.");
                    }

                    var expression = ExpressionParser.Parse(script, function, expr, literals, function.ReturnType);

                    return new ReturnStatement() { Expression = expression };
                }
            }
            else if (line == "break")
            {
                return new BreakStatement();
            }
            else if (line == "continue")
            {
                return new ContinueStatement();
            }
            else if (line.StartsWith("if "))
            {
                var expr = line[2..].Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("If statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals, Primitives.Bool);

                    return new IfStatement() { Condition = expression };
                }
            }
            else if (line.StartsWith("elif "))
            {
                var expr = line[4..].Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("Elif statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals, Primitives.Bool);

                    return new ElifStatement() { Condition = expression };
                }
            }
            else if (line == "else")
            {
                return new ElifStatement() { Condition = ConstExpression.True };
            }
            else if (line == "endif")
            {
                return new EndIfStatement();
            }
            else if (line.StartsWith("while "))
            {
                var expr = line[5..].Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("While statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals, Primitives.Bool);

                    return new WhileStatement() { Condition = expression };
                }
            }
            else if (line == "endwhile")
            {
                return new EndWhileStatement();
            }
            else if (line.StartsWith("for "))
            {
                var pieces = line.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                if (pieces.Count != 3)
                {
                    throw new Exception("Invalid for statement.");
                }

                var variable = script.GlobalVariables.Values.Concat(function.AllVariables)
                    .SingleOrDefault(x => x.Name == pieces[1]);

                if (variable is null)
                {
                    throw new Exception("Can not find for loop variable.");
                }

                var range = pieces[2].Split("..").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                if (range.Count != 2)
                {
                    throw new Exception("Invalid for range.");
                }

                var from = ExpressionParser.Parse(script, function, range[0], literals, Primitives.Int);
                var to = ExpressionParser.Parse(script, function, range[1], literals, Primitives.Int);

                var fs = new ForStatement()
                {
                    Variable = variable,
                    From = from,
                    To = to
                };

                fs.Validate();

                return fs;
            }
            else if (line == "endfor")
            {
                return new EndForStatement();
            }
            else
            {
                // assign statement

                var eq_pos = line.IndexOf('=');
                var lhs = eq_pos != -1 ? line[..eq_pos].Trim() : string.Empty;
                var rhs = line[(eq_pos + 1)..].Trim();
                Accessor? accessor = null;

                if (!string.IsNullOrWhiteSpace(lhs))
                {
                    if (ExpressionParser.TryParseAccessor(script, function, lhs, literals, out var a))
                    {
                        accessor = a;
                    }
                    else
                    {
                        throw new Exception($"Failed to parse assign statement: {line}");
                    }
                }

                var expression = ExpressionParser.Parse(script, function, rhs, literals, accessor?.Type ?? Primitives.Void);

                var statement = new AssignStatement()
                {
                    Accessor = accessor,
                    Expression = expression
                };

                statement.Validate();

                return statement;
            }
        }
    }
}
