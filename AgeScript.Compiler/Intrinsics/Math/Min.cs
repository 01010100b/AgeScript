﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Math
{
    internal class Min : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath(result, "min", cl, result_address, ref_result_address);
        }
    }

    internal class MinPrecise : Min
    {
        protected override Type ParameterType => Primitives.Precise;

        public MinPrecise() : base()
        {
            Name = "Min";
        }
    }
}
