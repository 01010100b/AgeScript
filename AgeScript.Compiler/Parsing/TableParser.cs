using AgeScript.Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Parsing
{
    internal class TableParser
    {
        public Table Parse(List<string> lines)
        {
            var name = lines[0].Replace("Table ", "").Replace(":", "").Trim();
            var lookup = new Table() { Name = name };

            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i].Trim();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (int.TryParse(line, out var val))
                    {
                        lookup.Values.Add(val);
                    }
                    else
                    {
                        throw new Exception("Failed to parse lookup.");
                    }
                }
            }

            lookup.Validate();

            return lookup;
        }
    }
}
