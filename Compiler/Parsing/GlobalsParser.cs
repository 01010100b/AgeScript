using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Parsing
{
    internal class GlobalsParser
    {
        public void Parse(Script script, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (line != "Globals:")
                {
                    var pieces = line.Split(' ');
                    var type_name = pieces[0].Trim();
                    var type = script.Types[type_name];
                    var name = pieces[1].Trim();
                    var variable = new Variable() { Name = name, Type = type };
                    variable.Validate();
                    script.GlobalVariables.Add(name, variable);
                }
            }
        }
    }
}
