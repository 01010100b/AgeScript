﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Language
{
    internal abstract class Named : Validated
    {
        private const string RegexString = @"^[a-zA-Z][a-zA-Z_0-9]*$";

        public required string Name { get; init; }

        public override void Validate()
        {
            var regex = RegexString;

            if (!Regex.IsMatch(Name, regex))
            {
                throw new Exception($"Name {Name} does not follow rules.");
            }

            if (Primitives.Keywords.Contains(Name))
            {
                throw new Exception($"Name {Name} is a reserved keyword.");
            }
        }
    }
}