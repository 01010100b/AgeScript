﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Units
{
    internal class ResetScouts : Inlined
    {
        public override bool HasStringLiteral => false;

        public ResetScouts() : base()
        {
            ReturnType = Primitives.Void;
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            result.Rules.AddAction("up-reset-scouts");
        }
    }
}
