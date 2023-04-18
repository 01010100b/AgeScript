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
        private static readonly Regex REGEX_GLOBALS = new("^Globals$");
        private static readonly Regex REGEX_TYPE = new("^Struct .*$");
        private static readonly Regex REGEX_FUNCTION = new("^Function .*$");

        public Script Parse(IEnumerable<string> lines)
        {
            var script = new Script();
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

            var globals_parser = new GlobalsParser();
            
            foreach (var code in globals_codes)
            {
                code.RemoveAt(0);
                globals_parser.Parse(script, code);
            }

            var function_parser = new FunctionParser();
            var bodies = new Dictionary<Function, List<string>>();

            foreach (var code in function_codes)
            {
                var function = function_parser.ParseHeader(script, code[0]);
                code.RemoveAt(0);
                bodies.Add(function, code);
            }

            foreach (var body in bodies)
            {
                function_parser.ParseBody(script, body.Key, body.Value);
            }

            script.Validate();

            return script;
        }
    }
}
