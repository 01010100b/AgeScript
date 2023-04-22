using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using AgeScript.Language.Statements;

namespace AgeScript.Parsing
{
    internal class StatementParser
    {
        private ExpressionParser ExpressionParser { get; } = new();

        public Statement Parse(Script script, Function function, string line,
            IReadOnlyDictionary<string, string> literals)
        {
            if (line == "return" || line.StartsWith("return "))
            {
                var expr = line.Replace("return", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    return new ReturnStatement() { Expression = null };
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

                    return new ReturnStatement() { Expression = expression };
                }
            }
            else if (line.StartsWith("if(") || line.StartsWith("if "))
            {
                var expr = line.Replace("if", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("If statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

                    return new IfStatement() { Expression = expression };
                }
            }
            else if (line.StartsWith("elif(") || line.StartsWith("elif "))
            {
                var expr = line.Replace("elif", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("Elif statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

                    return new ElifStatement() { Expression = expression };
                }
            }
            else if (line == "else")
            {
                return new ElifStatement() { Expression = ConstExpression.True };
            }
            else if (line == "endif")
            {
                return new EndIfStatement();
            }
            else
            {
                // assign statement

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

                var expression = ExpressionParser.Parse(script, function, rhs, literals);

                var statement = new AssignStatement()
                {
                    Variable = variable,
                    Offset = offset,
                    Type = type,
                    Expression = expression
                };

                statement.Validate();

                return statement;
            }
        }
    }
}
