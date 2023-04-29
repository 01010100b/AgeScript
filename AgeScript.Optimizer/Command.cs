using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public class Command
    {
        private static readonly HashSet<string> JumpCommands = new() { "up-jump-direct", "up-jump-rule", "up-jump-dynamic" };
        
        public string Code { get; set; } = string.Empty;
        public int Elements => Code.Count(x => x == '(');
        public bool IsCompound => Elements > 1;
        public bool IsJump => JumpCommands.Contains(GetId());

        private string GetId()
        {
            if (IsCompound)
            {
                throw new Exception("Can not get id of compound command");
            }

            var id = Code.Split(' ')[0].Replace("(", string.Empty).Replace(")", string.Empty).Trim();

            return id;
        }
    }
}
