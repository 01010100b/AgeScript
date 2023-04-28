using AgeScript.Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Parsing
{
    public class ScriptParser
    {
        private static readonly Regex REGEX_DEFINE = new("^#define .* .*$");
        private static readonly Regex REGEX_GLOBALS = new("^Globals:$");
        private static readonly Regex REGEX_STRUCT = new("^Struct .*:$");
        private static readonly Regex REGEX_FUNCTION = new("^Function .*:$");
        private static readonly Regex REGEX_TABLE = new("^Table .*:$");

        private VariableParser VariableParser { get; } = new();
        private FunctionParser FunctionParser { get; } = new();

        public Script Parse(List<string> lines)
        {
            var literals = new Dictionary<string, string>();
            lines = PreParse(lines, literals);

            var script = new Script();
            var globals_codes = new List<List<string>>();
            var struct_codes = new List<List<string>>();
            var function_codes = new List<List<string>>();
            var table_codes = new List<List<string>>();
            var current = new List<string>();

            foreach (var line in lines.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (REGEX_GLOBALS.IsMatch(line))
                {
                    current = new();
                    globals_codes.Add(current);
                }
                else if (REGEX_STRUCT.IsMatch(line))
                {
                    current = new();
                    struct_codes.Add(current);

                    throw new NotImplementedException();
                }
                else if (REGEX_FUNCTION.IsMatch(line))
                {
                    current = new();
                    function_codes.Add(current);
                }
                else if (REGEX_TABLE.IsMatch(line))
                {
                    current = new();
                    table_codes.Add(current);
                }

                current.Add(line);
            }

            var table_parser = new TableParser();

            foreach (var code in table_codes)
            {
                var table = table_parser.Parse(code);
                script.AddTable(table);
            }

            foreach (var code in globals_codes)
            {
                code.RemoveAt(0);

                foreach (var c in code)
                {
                    if (VariableParser.TryParseDefinition(script, c, out var v))
                    {
                        script.AddGlobal(v!);
                    }
                }
            }

            var bodies = new Dictionary<Function, List<string>>();

            foreach (var code in function_codes)
            {
                var function = FunctionParser.ParseHeader(script, code[0]);
                script.AddFunction(function);
                code.RemoveAt(0);
                bodies.Add(function, code);
            }

            foreach (var body in bodies)
            {
                FunctionParser.ParseBody(script, body.Key, body.Value, literals);
            }

            script.Validate();

            return script;
        }

        private List<string> PreParse(List<string> lines, Dictionary<string, string> literals)
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

                if (bo >= 0)
                {
                    var id = Guid.NewGuid().ToString().Replace("-", string.Empty);
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
