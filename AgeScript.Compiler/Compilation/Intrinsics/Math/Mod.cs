using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
{
    internal class Mod : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath(result, "mod", cl, result_address, ref_result_address);
        }
    }

    internal class ModPrecise : Mod
    {
        protected override Type ParameterType => Primitives.Precise;

        public ModPrecise() : base()
        {
            Name = "Mod";
        }
    }
}
