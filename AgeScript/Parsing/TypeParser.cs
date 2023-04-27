using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


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

                if (script.Types.TryGetValue(etn, out var etype))
                {
                    var cnt = pieces[1].Replace("]", "");

                    if (int.TryParse(cnt, out var length))
                    {
                        if (length > 0)
                        {
                            var at = new Array()
                            {
                                Name = code,
                                Size = etype.Size * length,
                                ElementType = etype,
                                Length = length
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
