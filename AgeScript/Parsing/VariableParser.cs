using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Parsing
{
    internal class VariableParser
    {
        private TypeParser TypeParser { get; } = new TypeParser();

        public bool TryParseDefinition(Script script, string code, out Variable? variable)
        {
            variable = null;

            var pieces = code.Trim().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            if (pieces.Count != 2)
            {
                return false;
            }

            var type_name = pieces[0].Trim();
            var name = pieces[1].Trim();

            if (TypeParser.TryParseType(script, type_name, out var type))
            {
                variable = new() { Name = name, Type = type! };
                variable.Validate();

                return true;
            }

            return false;
        }
    }
}
