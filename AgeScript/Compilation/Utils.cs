using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AgeScript.Language;

namespace AgeScript.Compilation
{
    internal static class Utils
    {
        private static string MemCopyAddressId { get; } = Script.GetUniqueId();

        public static void Clear(RuleList rules, int from, int length)
        {
            if (length <= 0)
            {
                return;
            }

            for (int i = 0; i < length; i++)
            {
                rules.AddAction($"set-goal {from + i} 0");
            }
        }

        public static void MemCopy(Script script, RuleList rules, int from, int to, int length,
            bool ref_from = false, bool ref_to = false,
            int from_offset = 0, int to_offset = 0, bool ref_from_offset = false, bool ref_to_offset = false)
        {
            if (length <= 0)
            {
                return;
            }

            if (ScriptCompiler.Settings.OptimizeMemCopy && length < ScriptCompiler.Settings.MaxElementsPerRule)
            {
                if (!ref_from && !ref_to && !ref_from_offset && !ref_to_offset)
                {
                    for (int i = 0; i < length; i++)
                    {
                        rules.AddAction($"up-modify-goal {to + to_offset + i} g:= {from + from_offset + i}");
                    }

                    return;
                }
            }

            // set pointers

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
                if (ref_from_offset)
                {
                    rules.AddAction($"up-modify-goal {script.Sp1} g:+ {from_offset}");
                }
                else
                {
                    rules.AddAction($"up-modify-goal {script.Sp1} c:+ {from_offset}");
                }
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
                if (ref_to_offset)
                {
                    rules.AddAction($"up-modify-goal {script.Sp2} g:+ {to_offset}");
                }
                else
                {
                    rules.AddAction($"up-modify-goal {script.Sp2} c:+ {to_offset}");
                }
            }

            // loop

            if (ScriptCompiler.Settings.OptimizeMemCopy && length <= ScriptCompiler.Settings.MaxElementsPerRule / 3)
            {
                // unroll if small length

                for (int i = 0; i < length; i++)
                {
                    rules.AddAction($"up-get-indirect-goal g: {script.Sp1} {script.Sp3}");
                    rules.AddAction($"up-set-indirect-goal g: {script.Sp2} g: {script.Sp3}");

                    if (i < length - 1)
                    {
                        rules.AddAction($"up-modify-goal {script.Sp1} c:+ 1");
                        rules.AddAction($"up-modify-goal {script.Sp2} c:+ 1");
                    }
                }

                return;
            }

            rules.AddAction($"set-goal {script.Sp0} {length}");

            if (ScriptCompiler.Settings.InlineMemCopy)
            {
                rules.StartNewRule($"up-compare-goal {script.Sp0} c:> 0");
                rules.AddAction($"up-get-indirect-goal g: {script.Sp1} {script.Sp3}");
                rules.AddAction($"up-set-indirect-goal g: {script.Sp2} g: {script.Sp3}");
                rules.AddAction($"up-modify-goal {script.Sp1} c:+ 1");
                rules.AddAction($"up-modify-goal {script.Sp2} c:+ 1");
                rules.AddAction($"up-modify-goal {script.Sp0} c:- 1");
                rules.AddAction("up-jump-rule -1");
                rules.StartNewRule();
            }
            else
            {
                var retid = Script.GetUniqueId();
                rules.AddAction($"set-goal {script.NonInlinedMemCopyReturnAddr} {retid}");
                rules.AddAction($"up-jump-direct c: {MemCopyAddressId}");
                rules.StartNewRule();
                rules.ReplaceStrings(retid, rules.CurrentRuleIndex.ToString());
            }
        }

        public static void CompileMemCopy(Script script, RuleList rules)
        {
            if (ScriptCompiler.Settings.InlineMemCopy)
            {
                return;
            }

            rules.StartNewRule($"up-compare-goal {script.Sp0} c:> 0");
            var jmpid = rules.CurrentRuleIndex;
            rules.AddAction($"up-get-indirect-goal g: {script.Sp1} {script.Sp3}");
            rules.AddAction($"up-set-indirect-goal g: {script.Sp2} g: {script.Sp3}");
            rules.AddAction($"up-modify-goal {script.Sp1} c:+ 1");
            rules.AddAction($"up-modify-goal {script.Sp2} c:+ 1");
            rules.AddAction($"up-modify-goal {script.Sp0} c:- 1");
            rules.AddAction("up-jump-rule -1");

            rules.StartNewRule();
            rules.AddAction($"up-jump-direct g: {script.NonInlinedMemCopyReturnAddr}");
            rules.StartNewRule();
            rules.ReplaceStrings(MemCopyAddressId, jmpid.ToString());
        }
    }
}
