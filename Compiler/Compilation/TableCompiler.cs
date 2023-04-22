using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class TableCompiler
    {
        public static int Modulus => ScriptCompiler.Settings.MaxElementsPerRule - 2;

        public void Compile(Script script, RuleList rules, Table table)
        {
            rules.StartNewRule();
            table.Address = rules.CurrentRuleIndex;
            var els = 0;

            foreach (var value in table.Values)
            {
                rules.AddAction($"set-goal {script.TableResultBase + els} {value}");
                els++;

                if (els >= Modulus)
                {
                    rules.AddAction($"up-jump-direct g: {script.SpecialGoal}");
                    rules.StartNewRule();
                    els = 0;
                }
            }

            if (els > 0)
            {
                rules.AddAction($"up-jump-direct g: {script.SpecialGoal}");
                rules.StartNewRule();
            }
        }
    }
}
