using AgeScript.Compiler.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation
{
    internal class TableCompiler
    {
        public static int Modulus => ScriptCompiler.Settings.MaxElementsPerRule - 2;

        public void Compile(Script script, RuleList rules, Table table)
        {
            Console.WriteLine($"Compiling table {table.Name}");
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
