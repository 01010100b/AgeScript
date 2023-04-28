using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language
{
    public abstract class Named : Validated
    {
        private const string NameRegex = @"^[a-zA-Z][a-zA-Z_0-9]*$";

        public static void ValidateName(string name)
        {
            if (!Regex.IsMatch(name, NameRegex))
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
