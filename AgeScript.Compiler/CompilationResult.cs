using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    public class CompilationResult
    {
        public string JumpTargetPer => Rules.GetPer();
        public IReadOnlyDictionary<string, int> JumpTargets => Rules.GetJumpTargets();
        public int RuleCount => Rules.TotalRules;
        public int CommandCount => Rules.TotalElements;

        internal RuleList Rules { get; private init; }
        internal Memory Memory { get; private init; }
        internal Settings Settings { get; private init; }

        internal CompilationResult(Script script, Settings settings)
        {
            Rules = new(script, settings);
            Memory = new(script, Rules, settings);
            Settings = settings;
        }

        public void Validate()
        {
            foreach (var target in Rules.GetJumpTargets())
            {
                if (target.Value < 0)
                {
                    throw new Exception("Unresolved jump target.");
                }
            }
        }
    }
}
