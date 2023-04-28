﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
{
    internal class Add : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath2(result, "+", cl, result_address, ref_result_address);
        }
    }

    internal class AddPrecise : Add
    {
        protected override Type ParameterType => Primitives.Precise;

        public AddPrecise() : base()
        {
            Name = "Add";
        }
    }
}
