﻿using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Statements
{
    public class ReturnStatement : Statement
    {
        public required Expression? Expression { get; init; }

        public override void Validate()
        {
            Expression?.Validate();
        }
    }
}
