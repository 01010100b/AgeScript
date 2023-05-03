using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

                if (s is ReturnStatement rs)
                {
                    if (rs.Expression is null && ReturnType != Primitives.Void)
                    {
                        throw new Exception("return statement must return value.");
                    }
                    else if (rs.Expression is not null && ReturnType == Primitives.Void)
                    {
                        throw new Exception("can not return values from function with return type Void.");
                    }
                    else if (rs.Expression is not null && ReturnType != Primitives.Void)
                    {
                        if (rs.Expression.Type != ReturnType)
                        {
                            throw new Exception("The type being returned does not match the declared return type.");
                        }
                    }
                }
            }

            if (Statements.OfType<IfStatement>().Count() != Statements.OfType<EndIfStatement>().Count())
            {
                throw new Exception("Mismatch between if and endif statements.");
            }
            else if (Statements.OfType<WhileStatement>().Count() != Statements.OfType<EndWhileStatement>().Count())
            {
                throw new Exception("Mismatch between while and endwhile statements.");
            }
            else if (Statements.OfType<ForStatement>().Count() != Statements.OfType<EndForStatement>().Count())
            {
                throw new Exception("Mismatch between for and endfor statements.");
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
