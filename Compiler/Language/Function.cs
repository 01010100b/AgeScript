using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Language.Statements;

namespace Compiler.Language
{
    internal class Function : Named
    {
        public Type ReturnType { get; set; } = Primitives.Void;
        public List<Variable> Parameters { get; } = new();
        public List<Variable> LocalVariables { get; } = new();
        public List<Statement> Statements { get; } = new();
        public IEnumerable<Variable> AllVariables => Parameters.Concat(LocalVariables);

        internal int RegisterCount => AllVariables.Sum(x => x.Type.Size) + 1; // add 1 because first register is return addr
        internal int Address { get; set; }
        internal string AddressableName { get; } = Guid.NewGuid().ToString().Replace("-", "");

        public bool TryGetScopedVariable(Script script, string name, out Variable? variable)
        {
            variable = default;

            if (script.GlobalVariables.TryGetValue(name, out var v))
            {
                variable = v;
            }
            else if (AllVariables.Any(x => x.Name == name))
            {
                variable = AllVariables.Single(x => x.Name == name);
            }

            if (variable is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Validate()
        {
            base.Validate();

            foreach (var v in AllVariables)
            {
                v.Validate();
            }

            foreach (var s in Statements)
            {
                s.Validate();
            }

            if (Statements.OfType<IfStatement>().Count() != Statements.OfType<EndIfStatement>().Count())
            {
                throw new Exception("Mismatch between if and endif statements.");
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Function f)
            {
                if (Name != f.Name || Parameters.Count != f.Parameters.Count)
                {
                    return false;
                }

                for (int i = 0; i < Parameters.Count; i++)
                {
                    var t1 = Parameters[i].Type;
                    var t2 = f.Parameters[i].Type;

                    if (!t1.Equals(t2))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            hash ^= Parameters.Count.GetHashCode();

            foreach (var type in Parameters.Select(x => x.Type))
            {
                hash ^= type.GetHashCode();
            }

            return hash;
        }
    }
}
