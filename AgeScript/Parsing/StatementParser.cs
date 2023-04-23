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
            else if (line.StartsWith("if "))
            {
                var expr = line.Replace("if", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("If statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

                    return new IfStatement() { Condition = expression };
                }
            }
            else if (line.StartsWith("elif "))
            {
                var expr = line.Replace("elif", "").Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("Elif statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

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
                var expr = line.Replace("while", string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(expr))
                {
                    throw new Exception("While statement needs expression.");
                }
                else
                {
                    var expression = ExpressionParser.Parse(script, function, expr, literals);

                    return new WhileStatement() { Condition = expression };
                }
            }
            else if (line == "endwhile")
            {
                return new EndWhileStatement();
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
                    if (ExpressionParser.TryParseAccessor(script, function, lhs, out var a))
                    {
                        accessor = a;
                    }
                }

                var expression = ExpressionParser.Parse(script, function, rhs, literals);

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
