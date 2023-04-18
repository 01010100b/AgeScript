using Compiler.Compilation.Intrinsics;
using Compiler.Language.Expressions;
using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void MemCopy(RuleList rules, int from, int to, int length, 
            bool ref_from = false, bool ref_to = false, int from_offset = 0, int to_offset = 0)
        {
            rules.AddAction($"set-goal sp0 {length}");

            if (ref_from)
            {
                rules.AddAction($"up-modify-goal sp1 g:= {from}");
            }
            else
            {
                rules.AddAction($"set-goal sp1 {from}");
            }

            if (from_offset != 0)
            {
                rules.AddAction($"up-modify-goal sp1 c:+ {from_offset}");
            }

            if (ref_to)
            {
                rules.AddAction($"up-modify-goal sp2 g:= {to}");
            }
            else
            {
                rules.AddAction($"set-goal sp2 {to}");
            }

            if (to_offset != 0)
            {
                rules.AddAction($"up-modify-goal sp2 c:+ {to_offset}");
            }

            rules.StartNewRule("up-compare-goal sp0 c:> 0");
            rules.AddAction("up-get-indirect-goal g: sp1 sp3");
            rules.AddAction("up-set-indirect-goal g: sp2 g: sp3");
            rules.AddAction("up-modify-goal sp1 c:+ 1");
            rules.AddAction("up-modify-goal sp2 c:+ 1");
            rules.AddAction("up-modify-goal sp0 c:- 1");
            rules.AddAction("up-jump-rule -1");
            rules.StartNewRule();
        }
    }
}
