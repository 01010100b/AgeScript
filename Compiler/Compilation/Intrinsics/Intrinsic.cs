using Compiler.Language;
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
        public static IEnumerable<Intrinsic> GetIntrinsics(Script script)
        {
            var assembly = typeof(Intrinsic).Assembly;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(Intrinsic)) && !type.IsAbstract)
                {
                    var instance = Activator.CreateInstance(type, script);

                    if (instance is not null)
                    {
                        yield return (Intrinsic)instance;
                    }
                }
            }
        }

        public Intrinsic(Script script)
        {

        }

        public abstract void Compile(RuleList rules, CallExpression cl, int? address);
    }
}
