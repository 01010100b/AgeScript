﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language
{
    internal class Table : Named
    {
        public List<int> Values { get; } = new();

        internal int Address { get; set; }
        internal string AddressableName { get; } = Guid.NewGuid().ToString().Replace("-", "");

        public override bool Equals(object? obj) => obj is Table l && Name.Equals(l.Name);
        public override int GetHashCode() => Name.GetHashCode();

        public override void Validate()
        {
            base.Validate();
        }
    }
}
