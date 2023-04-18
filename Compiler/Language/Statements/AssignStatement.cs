﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Language.Expressions;

namespace Compiler.Language.Statements
{
    internal class AssignStatement : Statement
    {
        public required Variable? Variable { get; init; }
        public required int Offset { get; init; }
        public required Type Type { get; init; }
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            Variable?.Validate();
            Type.Validate();
            Expression.Validate();
        }
    }
}