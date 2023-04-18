using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class ScriptCompiler
    {
        public RuleList Compile(Script script)
        {
            script.Validate();

            var rules = new RuleList();

            var mem = new MemoryCompiler();
            mem.Compile(script, rules);

            var func = new FunctionCompiler();
            script.Functions.Sort((a, b) =>
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

            foreach (var function in script.Functions)
            {
                func.Compile(script, function, rules);
            }

            foreach (var function in script.Functions)
            {
                rules.ReplaceStrings(function.AddressableName, function.Address.ToString());
            }

            return rules;
        }
    }
}
