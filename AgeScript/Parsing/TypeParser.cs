using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Array = AgeScript.Language.Array;
using Type = AgeScript.Language.Type;

namespace AgeScript.Parsing
{
    internal class TypeParser
    {
        private const string ARRAY_REGEX = @"^[a-zA-Z_0-9]+\[[0-9]+\]$";

        public bool TryParseType(Script script, string code, out Type? type)
        {
            type = null;

            if (script.Types.TryGetValue(code, out var t))
            {
                type = t;

                return true;
            }
            else if (Regex.IsMatch(code, ARRAY_REGEX))
            {
                var pieces = code.Split('[');
                var etn = pieces[0].Trim();

                if (script.Types.TryGetValue(etn, out var et))
                {
                    var cnt = pieces[1].Replace("]", "");

                    if (int.TryParse(cnt, out var lt))
                    {
                        if (lt > 0)
                        {
                            var at = new Array()
                            {
                                Name = code,
                                Size = et.Size * lt,
                                ElementType = et,
                                Length = lt
                            };

                            at.Validate();
                            script.AddType(at);

                            type = at;

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
