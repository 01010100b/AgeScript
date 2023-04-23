using AgeScript.Compilation.Intrinsics;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgeScript.Language
{
    public class Script : Validated
    {
        public IReadOnlyDictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
        public IReadOnlyDictionary<string, Variable> GlobalVariables { get; } = new Dictionary<string, Variable>();
        public IEnumerable<Function> Functions { get; } = new List<Function>();
        public IEnumerable<Table> Tables { get; } = new List<Table>();

        internal int SpecialGoal { get; set; } // used for control-flow (if, ...) conditions and table lookup return addresses
        internal int StackPtr { get; set; } // points to next free stack goal
        internal int Sp0 { get; set; } // special purpose registers, for memcopy and such
        internal int Sp1 { get; set; }
        internal int Sp2 { get; set; }
        internal int Sp3 { get; set; }
        internal int Intr0 { get; set; } // special registers for intrinsics
        internal int Intr1 { get; set; }
        internal int Intr2 { get; set; }
        internal int Intr3 { get; set; }
        internal int RegisterBase { get; set; } // start of registers
        internal int RegisterCount { get; set; } // number of registers
        internal int CallResultBase { get; set; } // start of goals where result of a function call is stored
        internal int TableResultBase { get; set; } // start of lookup result

        public Script()
        {
            foreach (var type in Primitives.Types)
            {
                AddType(type);
            }

            var assembly = typeof(Intrinsic).Assembly;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(Intrinsic)) && !type.IsAbstract)
                {
                    var instance = Activator.CreateInstance(type);

                    if (instance is not null)
                    {
                        AddFunction((Intrinsic)instance);
                    }
                }
            }
        }

        public void AddTable(Table table)
        {
            table.Validate();

            if (Tables.Contains(table))
            {
                throw new Exception("Already have table.");
            }

            ((List<Table>)Tables).Add(table);
        }

        public void AddType(Type type)
        {
            type.Validate();

            if (Types.Values.Contains(type))
            {
                throw new Exception("Already have type.");
            }

            ((Dictionary<string, Type>)Types).Add(type.Name, type);
        }

        public void AddGlobal(Variable global)
        {
            global.Validate();

            if (GlobalVariables.Values.Contains(global))
            {
                throw new Exception("Already have global variable.");
            }

            ((Dictionary<string, Variable>)GlobalVariables).Add(global.Name, global);
        }

        public void AddFunction(Function function)
        {
            function.Validate();

            if (Functions.Contains(function))
            {
                throw new Exception($"Already have function {function.Name}.");
            }

            ((List<Function>)Functions).Add(function);
        }

        public bool TryGetFunction(string name, IReadOnlyList<Expression> arguments, string? literal, out Function? function)
        {
            var f = Functions.SingleOrDefault(x =>
            {
                if (x.Name != name || x.Parameters.Count != arguments.Count)
                {
                    return false;
                }
                else
                {
                    if (x is Intrinsic intr)
                    {
                        if (intr.HasStringLiteral == literal is null)
                        {
                            return false;
                        }
                    }

                    for (int i = 0; i < arguments.Count; i++)
                    {
                        var a = arguments[i];
                        var p = x.Parameters[i];

                        if (a.Type != p.Type)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            });

            if (f is not null)
            {
                function = f;

                return true;
            }
            else
            {
                function = default;

                return false;
            }
        }

        public override void Validate()
        {
            if (Functions.Count(x => x.Name == "Main") != 1)
            {
                throw new Exception("Must have a unique Main function.");
            }

            var main = Functions.Single(x => x.Name == "Main");

            if (main.ReturnType != Primitives.Void)
            {
                throw new Exception("Main function must have return type Void.");
            }

            if (main.Parameters.Count != 0)
            {
                throw new Exception("Main function must have 0 parameters.");
            }

            var types = new HashSet<string>();

            foreach (var type in Types.Values)
            {
                type.Validate();

                if (types.Contains(type.Name))
                {
                    throw new Exception("Already have type with this name.");
                }

                types.Add(type.Name);
            }

            var globals = new HashSet<string>();

            foreach (var global in GlobalVariables.Values)
            {
                global.Validate();

                if (globals.Contains(global.Name))
                {
                    throw new Exception("Already have global name.");
                }

                globals.Add(global.Name);
            }

            var functions = new HashSet<Function>();

            foreach (var function in Functions)
            {
                if (functions.Contains(function))
                {
                    throw new Exception("Function defined more than once.");
                }

                function.Validate();
                functions.Add(function);

                foreach (var v in function.AllVariables)
                {
                    if (globals.Contains(v.Name))
                    {
                        throw new Exception("Have global with same name as local.");
                    }
                }
            }
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
