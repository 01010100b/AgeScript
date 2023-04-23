﻿using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Math
{
    internal class Sub : MathIntrinsic
    {
        protected override Language.Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileMath(script, function, rules, "-", cl, result_address, ref_result_address);
        }
    }

    internal class SubPrecise : Sub
    {
        protected override Language.Type ParameterType => Primitives.Precise;

        public SubPrecise() : base()
        {
            Name = "Sub";
        }
    }
}