using Compiler.Compilation.Intrinsics;
using Compiler.Language.Expressions;
using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Compiler.Compilation
{
    internal static class Utils
    {
        public static void Clear(RuleList rules, int from, int length)
        {
            for (int i = 0; i < length; i++)
            {
                rules.AddAction($"set-goal {from + i} 0");
            }
        }

        public static void MemCopy(Script script, RuleList rules, int from, int to, int length, 
            bool ref_from = false, bool ref_to = false, int from_offset = 0, int to_offset = 0)
        {
            if (length < Program.Settings.MaxElementsPerRule)
            {
                if (!ref_from && !ref_to)
                {
                    MemCopyDirect(script, rules, from, to, length, from_offset, to_offset);

                    return;
                }
            }

            rules.AddAction($"set-goal {script.Sp0} {length}");

            if (ref_from)
            {
                rules.AddAction($"up-modify-goal {script.Sp1} g:= {from}");
            }
            else
            {
                rules.AddAction($"set-goal {script.Sp1} {from}");
            }

            if (from_offset != 0)
            {
                rules.AddAction($"up-modify-goal {script.Sp1} c:+ {from_offset}");
            }

            if (ref_to)
            {
                rules.AddAction($"up-modify-goal {script.Sp2} g:= {to}");
            }
            else
            {
                rules.AddAction($"set-goal {script.Sp2} {to}");
            }

            if (to_offset != 0)
            {
                rules.AddAction($"up-modify-goal {script.Sp2} c:+ {to_offset}");
            }

            rules.StartNewRule($"up-compare-goal {script.Sp0} c:> 0");
            rules.AddAction($"up-get-indirect-goal g: {script.Sp1} {script.Sp3}");
            rules.AddAction($"up-set-indirect-goal g: {script.Sp2} g: {script.Sp3}");
            rules.AddAction($"up-modify-goal {script.Sp1} c:+ 1");
            rules.AddAction($"up-modify-goal {script.Sp2} c:+ 1");
            rules.AddAction($"up-modify-goal {script.Sp0} c:- 1");
            rules.AddAction("up-jump-rule -1");
            rules.StartNewRule();
        }

        private static void MemCopyDirect(Script script, RuleList rules, int from, int to, int length,
            int from_offset, int to_offset)
        {
            for (int i = 0; i < length; i++)
            {
                rules.AddAction($"up-modify-goal {to + to_offset + i} g:= {from + from_offset + i}");
            }
        }
    }
}
