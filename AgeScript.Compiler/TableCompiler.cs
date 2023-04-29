using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    internal class TableCompiler
    {
        public void Compile(CompilationResult result, Table table)
        {
            Console.WriteLine($"Compiling table {table.Name}");
            result.Rules.StartNewRule();
            result.Rules.ResolveJumpTarget(result.Rules.GetJumpTarget(table));
            var element = 0;

            foreach (var value in table.Values)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.TableResultBase + element} c:= {value}");
                element++;

                if (element >= result.Settings.TableModulus)
                {
                    result.Rules.AddAction($"up-jump-direct g: {result.Memory.Intr2}");
                    result.Rules.StartNewRule();
                    element = 0;
                }
            }

            if (element > 0)
            {
                result.Rules.AddAction($"up-jump-direct g: {result.Memory.Intr2}");
                result.Rules.StartNewRule();
            }
        }
    }
}
