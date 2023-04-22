using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    public class ScriptCompiler
    {
        internal static Settings Settings { get; private set; } = new();

        private static readonly object Lock = new();

        public RuleList Compile(Script script, Settings settings)
        {
            script.Validate();
            var rules = new RuleList();

            lock (Lock)
            {
                Settings = settings;
                Compile(script, rules);
            }

            return rules;
        }

        private void Compile(Script script, RuleList rules)
        {
            // layout memory

            var memory_compiler = new MemoryCompiler();
            memory_compiler.Compile(script, rules);

            // compile code

            var function_compiler = new FunctionCompiler();

            var functions = script.Functions.ToList();
            functions.Sort((a, b) =>
            {
                if (a.Name == "Main")
                {
                    return -1;
                }
                else if (b.Name == "Main")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            foreach (var function in functions)
            {
                function_compiler.Compile(script, function, rules);
            }

            var table_compiler = new TableCompiler();

            foreach (var table in script.Tables)
            {
                table_compiler.Compile(script, rules, table);
            }

            // linker

            foreach (var function in functions)
            {
                rules.ReplaceStrings(function.AddressableName, function.Address.ToString());
            }

            foreach (var table in script.Tables)
            {
                rules.ReplaceStrings(table.AddressableName, table.Address.ToString());
            }
        }
    }
}
