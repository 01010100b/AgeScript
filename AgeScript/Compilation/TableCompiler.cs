using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    internal class TableCompiler
    {
        public static int Modulus => ScriptCompiler.Settings.MaxElementsPerRule - 2;

        public void Compile(Script script, RuleList rules, Table table)
        {
            rules.StartNewRule();
            table.Address = rules.CurrentRuleIndex;
            var elements = 0;

            foreach (var value in table.Values)
            {
                rules.AddAction($"set-goal {script.TableResultBase + elements} {value}");
                elements++;

                if (elements >= Modulus)
                {
                    rules.AddAction($"up-jump-direct g: {script.SpecialGoal}");
                    rules.StartNewRule();
                    elements = 0;
                }
            }

            if (elements > 0)
            {
                rules.AddAction($"up-jump-direct g: {script.SpecialGoal}");
                rules.StartNewRule();
            }
        }
    }
}
