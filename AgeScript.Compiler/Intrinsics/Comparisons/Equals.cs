﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Comparisons
{
    internal class Equals : ComparisonIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileComparison(result, "==", cl, result_address, ref_result_address);
        }
    }

    internal class EqualsPrecise : Equals
    {
        protected override Type ParameterType => Primitives.Precise;

        public EqualsPrecise() : base()
        {
            Name = "Equals";
        }
    }

    internal class EqualsBool : Equals
    {
        protected override Type ParameterType => Primitives.Bool;

        public EqualsBool() : base()
        {
            Name = "Equals";
        }
    }
}
