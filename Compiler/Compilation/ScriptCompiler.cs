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
            var rules = new RuleList();

            var mem = new MemoryCompiler();
            mem.Compile(script, rules);

            var func = new FunctionCompiler();
            
            foreach (var function in script.Functions)
            {
                func.Compile(script, function, rules);
            }

            return rules;
        }
    }
}
