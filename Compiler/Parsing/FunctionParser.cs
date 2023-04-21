using Compiler.Language;
using Compiler.Language.Expressions;
using Compiler.Language.Statements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Parsing
{
    internal class FunctionParser
    {
        private TypeParser TypeParser { get; } = new();
        private VariableParser VariableParser { get; } = new();

        private Dictionary<string, string> Literals { get; } = new();

        public Function ParseHeader(Script script, string header)
        {
            // name, return type, and parameters

            var bo_pos = header.IndexOf("(");
            var bc_pos = header.IndexOf(")");

            if (bo_pos == -1 || bc_pos == -1 || bc_pos <= bo_pos)
            {
                throw new Exception("Failed to parse function header.");
            }

            var def = header[..bo_pos];
            var pars = header[(bo_pos + 1)..bc_pos];
            var defpieces = def.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var parspieces = pars.Split(',');

            if (!TypeParser.TryParseType(script, defpieces[1].Trim(), out var type))
            {
                throw new Exception("Can not parse return type.");
            }

            var name = defpieces[2].Trim();
            var function = new Function() { Name = name, ReturnType = type! };

            foreach (var par in parspieces.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (VariableParser.TryParseDefinition(script, par, out var v))
                {
                    function.Parameters.Add(v!);
                }
                else
                {
                    throw new Exception("Failed to parse parameter.");
                }
            }

            if (script.Functions.Contains(function))
            {
                throw new Exception("Function already defined.");
            }

            return function;
        }

        public void ParseBody(Script script, Function function, IEnumerable<string> lines)
        {
            Literals.Clear();

            // variables and statements

            foreach (var line in lines)
            {
                var pieces = line.Split(' ');

                if (pieces.Length == 2 && script.Types.TryGetValue(pieces[0].Trim(), out var type))
                {
                    var name = pieces[1].Trim();

                    if (function.TryGetScopedVariable(script, name, out _))
                    {
                        throw new Exception($"A variable with name {name} already exists.");
                    }

                    var variable = new Variable() { Name = name, Type = type };
                    variable.Validate();
                    function.LocalVariables.Add(variable);
                }
                else
                {
                    var statement = ParseStatement(script, function, line);
                    statement.Validate();
                    function.Statements.Add(statement);
                }
            }

        }

        private Statement ParseStatement(Script script, Function function, string line)
        {
            if (line == "return" || line.StartsWith("return "))
            {
                // return statement

                var expr = line.Replace("return", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    return new ReturnStatement() { Expression = null };
                }
                else
                {
                    var expression = ParseExpression(script, function, expr);

                    return new ReturnStatement() { Expression = expression };
                }
            }
            else if (line.StartsWith("if(") || line.StartsWith("if "))
            {
                // if statement

                var expr = line.Replace("if", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("If statement needs expression.");
                }
                else
                {
                    var expression = ParseExpression(script, function, expr);
                    expression.Validate();

                    return new IfStatement() { Expression = expression };
                }
            }
            else if (line.StartsWith("elif(") || line.StartsWith("elif "))
            {
                // elif statement

                var expr = line.Replace("elif", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("Elif statement needs expression.");
                }
                else
                {
                    var expression = ParseExpression(script, function, expr);
                    expression.Validate();

                    return new ElifStatement() { Expression = expression };
                }
            }
            else if (line == "else")
            {
                // else statement

                var expression = new ConstExpression("true");
                expression.Validate();

                return new ElifStatement() { Expression = expression };
            }
            else if (line == "endif")
            {
                // endif statement

                return new EndIfStatement();
            }
            else
            {
                // assign statement

                if (line.Contains('\"'))
                {
                    // has string literal

                    var lits = line.IndexOf('\"');
                    var lite = line.LastIndexOf('\"');
                    var literal = line[lits..(lite + 1)];
                    var id = Guid.NewGuid().ToString().Replace("-", "");
                    line = line.Replace(literal, id);
                    Literals.Add(id, literal);
                }

                var eq_pos = line.IndexOf('=');
                var lhs = eq_pos != -1 ? line[..eq_pos].Trim() : string.Empty;
                var rhs = line[(eq_pos + 1)..].Trim();

                Variable? variable = null;
                int offset = 0;
                var type = Primitives.Void;

                if (!string.IsNullOrWhiteSpace(lhs))
                {
                    if (function.TryGetScopedVariable(script, lhs, out var v))
                    {
                        variable = v!;
                        type = variable.Type;
                    }
                    else
                    {
                        throw new Exception("Variable to assign to not found.");
                    }
                }

                var expression = ParseExpression(script, function, rhs);

                var statement = new AssignStatement()
                {
                    Variable = variable,
                    Offset = offset,
                    Type = type,
                    Expression = expression
                };

                return statement;
            }
        }

        private Expression ParseExpression(Script script, Function function, string expression)
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

                    if (Literals.ContainsKey(arg))
                    {
                        if (i == 0)
                        {
                            literal = Literals[arg];
                        }
                        else
                        {
                            throw new Exception("String literals can only be first argument.");
                        }
                    }
                    else
                    {
                        var expr = ParseExpression(script, function, arg);

                        if (expr is CallExpression)
                        {
                            throw new Exception("Can not nest call expressions.");
                        }

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

                return cex;
            }
            else if (function.TryGetScopedVariable(script, expression, out var variable))
            {
                // variable expression

                return new VariableExpression() { Variable = variable!, Offset = 0, ElementType = variable!.Type };
            }
            else
            {
                // const expression

                return new ConstExpression(expression);
            }

            throw new NotImplementedException();
        }
    } 
}
