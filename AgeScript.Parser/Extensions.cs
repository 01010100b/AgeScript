using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Parser
{
    internal static class Extensions
    {
        public static List<string> SplitFull(this string input, string separator) => input.Trim().Split(separator)
            .Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }
}
