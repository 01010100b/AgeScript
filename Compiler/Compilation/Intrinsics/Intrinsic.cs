﻿using Compiler.Language;
using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation.Intrinsics
{
    internal abstract class Intrinsic : Function
    {
        public abstract bool HasStringLiteral { get; }

        public Intrinsic(Script script)
        {

        }

        public abstract void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? address);
    }
}
