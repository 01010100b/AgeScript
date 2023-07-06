using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeScript.Language
{
    public abstract class Named : Validated
    {
        private static readonly Regex NameRegex = new(@"^[a-zA-Z_][a-zA-Z_0-9]*$");

        public static void ValidateName(string name)
        {
            if (!NameRegex.IsMatch(name))
            {
                throw new Exception($"Name {name} does not follow rules.");
            }

            if (Primitives.Keywords.Contains(name))
            {
                throw new Exception($"Name {name} is a reserved keyword.");
            }
        }


        public required string Name { get; init; }

        public override void Validate() => ValidateName(Name);
    }
}
