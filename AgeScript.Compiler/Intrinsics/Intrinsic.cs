using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal abstract class Intrinsic : Function
    {
        public static IEnumerable<Intrinsic> Intrinsics { get; } = GetIntrinsics();

        public abstract bool HasStringLiteral { get; }
        protected ExpressionCompiler ExpressionCompiler { get; }

        public Intrinsic() : base()
        {
            Name = GetType().Name;
            ExpressionCompiler = new(this);
        }

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
