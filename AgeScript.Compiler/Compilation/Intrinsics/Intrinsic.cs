using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal abstract class Intrinsic : Function
    {
        public static IEnumerable<Intrinsic> Intrinsics { get; } = GetIntrinsics();

        public abstract bool HasStringLiteral { get; }

        protected ExpressionCompiler2 ExpressionCompiler2 { get; }

        public Intrinsic() : base()
        {
            Name = GetType().Name;
            ExpressionCompiler2 = new(this);
        }

        internal abstract void CompileCall2(CompilationResult result, CallExpression cl,
            int? result_address = null, bool ref_result_address = false);

        private static List<Intrinsic> GetIntrinsics()
        {
            var intrinsics = new List<Intrinsic>();
            var assembly = typeof(Intrinsic).Assembly;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(Intrinsic)) && !type.IsAbstract)
                {
                    var instance = Activator.CreateInstance(type);

                    if (instance is not null)
                    {
                        intrinsics.Add((Intrinsic)instance);
                    }
                }
            }

            return intrinsics;
        }
    }
}
