﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace AgeScript.Language
{
    public class Script : Validated
    {
        public IReadOnlyDictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
        public IReadOnlyDictionary<string, Variable> GlobalVariables { get; } = new Dictionary<string, Variable>();
        public IEnumerable<Function> Functions { get; } = new List<Function>();
        public IEnumerable<Table> Tables { get; } = new List<Table>();

        public Script()
        {
            foreach (var type in Primitives.Types)
            {
                AddType(type);
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

            var names = new HashSet<string>();

            foreach (var type in Types.Values)
            {
                type.Validate();

                if (names.Contains(type.Name))
                {
                    throw new Exception($"Type name {type.Name} already used.");
                }

                names.Add(type.Name);
            }

            foreach (var global in GlobalVariables.Values)
            {
                global.Validate();

                if (names.Contains(global.Name))
                {
                    throw new Exception($"Global name {global.Name} already used.");
                }

                names.Add(global.Name);
            }

            var functions = new HashSet<Function>();

            foreach (var function in Functions)
            {
                if (functions.Contains(function))
                {
                    throw new Exception("Function defined more than once.");
                }
                else if (names.Contains(function.Name))
                {
                    throw new Exception("Function name already used.");
                }

                foreach (var v in function.AllVariables)
                {
                    if (names.Contains(v.Name))
                    {
                        throw new Exception("Local variable name already used.");
                    }
                }

                function.Validate();
                functions.Add(function);
            }
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                {
                    Modifiers = { AddPrivateFieldsModifier }
                }
            };

            return JsonSerializer.Serialize(this, options);
        }

        private static void AddPrivateFieldsModifier(JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            foreach (FieldInfo field in jsonTypeInfo.Type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.Name.Contains("__BackingField"))
                {
                    continue;
                }

                JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(field.FieldType, field.Name);
                jsonPropertyInfo.Get = field.GetValue;
                jsonPropertyInfo.Set = field.SetValue;

                jsonTypeInfo.Properties.Add(jsonPropertyInfo);
            }

            foreach (var prop in jsonTypeInfo.Type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(prop.PropertyType, prop.Name);
                jsonPropertyInfo.Get = prop.GetValue;
                jsonPropertyInfo.Set = prop.SetValue;

                jsonTypeInfo.Properties.Add(jsonPropertyInfo);
            }
        }
    }
}
