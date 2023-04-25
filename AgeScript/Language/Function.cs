using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Language.Statements;

namespace AgeScript.Language
{
    public class Function : Named
    {
        public Type ReturnType { get; set; }
        public List<Variable> Parameters { get; } = new();
        public List<Variable> LocalVariables { get; } = new();
        public List<Statement> Statements { get; } = new();
        public IEnumerable<Variable> AllVariables => Parameters.Concat(LocalVariables);

        internal int RegisterCount => AllVariables.Sum(x => x.Type.Size) + 1; // add 1 because first register is return addr
        internal int Address { get; set; } = -1;
        internal string AddressableName { get; } = Script.GetUniqueId();

        public Function()
        {
            ReturnType = Primitives.Void;
        }

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

            var locals = new HashSet<string>();

            foreach (var v in AllVariables)
            {
                v.Validate();

                if (locals.Contains(v.Name))
                {
                    throw new Exception("Already have local variable.");
                }

                locals.Add(v.Name);
            }

            foreach (var s in Statements)
            {
                s.Validate();
            }

            if (Statements.OfType<IfStatement>().Count() != Statements.OfType<EndIfStatement>().Count())
            {
                throw new Exception("Mismatch between if and endif statements.");
            }
            else if (Statements.OfType<WhileStatement>().Count() != Statements.OfType<EndWhileStatement>().Count())
            {
                throw new Exception("Mismatch between while and endwhile statements.");
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

                    if (t1 != t2)
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
