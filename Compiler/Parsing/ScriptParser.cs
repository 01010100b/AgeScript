using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Parsing
{
    internal class ScriptParser
    {
        private static readonly Regex REGEX_DEFINE = new("^#define .* .*$");
        private static readonly Regex REGEX_GLOBALS = new("^Globals:$");
        private static readonly Regex REGEX_TYPE = new("^Struct .*:$");
        private static readonly Regex REGEX_FUNCTION = new("^Function .*:$");

        private VariableParser VariableParser { get; } = new();
        private FunctionParser FunctionParser { get; } = new();

        public Script Parse(List<string> lines)
        {
            var script = new Script();
            var literals = new Dictionary<string, string>();
            lines = PreParse(script, lines, literals);

            var globals_codes = new List<List<string>>();
            var type_codes = new List<List<string>>();
            var function_codes = new List<List<string>>();
            var current = new List<string>();

            foreach (var line in lines.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (REGEX_GLOBALS.IsMatch(line))
                {
                    current = new();
                    globals_codes.Add(current);
                }
                else if (REGEX_TYPE.IsMatch(line))
                {
                    current = new();
                    type_codes.Add(current);
                }
                else if (REGEX_FUNCTION.IsMatch(line))
                {
                    current = new();
                    function_codes.Add(current);
                }

                current.Add(line);
            }

            foreach (var code in globals_codes)
            {
                code.RemoveAt(0);

                foreach (var c in code)
                {
                    if (VariableParser.TryParseDefinition(script, c, out var v))
                    {
                        script.GlobalVariables.Add(v!.Name, v);
                    }
                }
            }

            var bodies = new Dictionary<Function, List<string>>();

            foreach (var code in function_codes)
            {
                var function = FunctionParser.ParseHeader(script, code[0]);
                script.Functions.Add(function);
                code.RemoveAt(0);
                bodies.Add(function, code);
            }

            foreach (var body in bodies)
            {
                FunctionParser.ParseBody(script, body.Key, body.Value);
            }

            script.Validate();

            return script;
        }

        private List<string> PreParse(Script script, List<string> lines, Dictionary<string, string> literals)
        {
            var res = new List<string>();
            var defines = new Dictionary<string, string>();

            foreach (var lineit in lines)
            {
                var line = lineit;

                var comment_pos = line.IndexOf("//");

                if (comment_pos >= 0)
                {
                    line = line[..comment_pos];
                }

                var bo = line.IndexOf("\"");
                var bc = line.LastIndexOf("\"");

                if (bo >= 0 && bc >= 0)
                {
                    var id = Guid.NewGuid().ToString().Replace("-", "");
                    var lit = line[bo..(bc + 1)];
                    literals.Add(id, lit);
                    line = line.Replace(lit, id);
                }

                line = line.Trim();

                if (REGEX_DEFINE.IsMatch(line))
                {
                    var pieces = line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                    if (pieces.Count != 3)
                    {
                        throw new Exception("Failed to parse define.");
                    }

                    defines.Add(pieces[1], pieces[2]);
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    res.Add(line);
                }
            }

            for (int i = 0; i < res.Count; i++)
            {
                var line = res[i];

                foreach (var kvp in defines)
                {
                    line = line.Replace(kvp.Key, kvp.Value);
                }

                res[i] = line;
            }

            return res;
        }
    }
}
