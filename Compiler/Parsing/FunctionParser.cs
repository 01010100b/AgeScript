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
        private StatementParser StatementParser { get; } = new();

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

        public void ParseBody(Script script, Function function, IEnumerable<string> lines, 
            IReadOnlyDictionary<string, string> literals)
        {
            // local variables and statements

            foreach (var line in lines)
            {
                var pieces = line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                if (pieces.Count == 2 && script.Types.TryGetValue(pieces[0].Trim(), out var type))
                {
                    var name = pieces[1].Trim();

                    if (function.TryGetScopedVariable(script, name, out _))
                    {
                        throw new Exception($"A variable with name {name} already exists in scope.");
                    }

                    if (VariableParser.TryParseDefinition(script, line, out var v))
                    {
                        function.LocalVariables.Add(v!);
                    }
                }
                else
                {
                    var statement = StatementParser.Parse(script, function, line, literals);
                    function.Statements.Add(statement);
                }
            }

        }
    } 
}
