using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRBasicBlock : HIRFunctionLocalValue
    {
        public HIRFunction? owner;
        public HIRBasicBlock? previousBlock;
        public HIRBasicBlock? nextBlock;

        public HIRInstruction? firstInstruction;
        public HIRInstruction? lastInstruction;

        public HIRBasicBlock(SourcePosition? sourcePosition, string? name) : base(sourcePosition, Intrinsics.GetSingleton().BasicBlockType, name)
        {
        }

        public void AddIntruction(HIRInstruction instruction)
        {
            Debug.Assert(instruction.owner == null);
            Debug.Assert(instruction.previousInstruction == null);
            Debug.Assert(instruction.nextInstruction == null);
            instruction.owner = this;

            if (lastInstruction != null)
            {
                lastInstruction.nextInstruction = instruction;
                instruction.previousInstruction = lastInstruction;
                lastInstruction = instruction;
            }
            else
            {
                firstInstruction = lastInstruction = instruction;
            }

        }

        public override void DumpToConsole()
        {
            Console.WriteLine(ToString() + ":");
            HIRInstruction? currentPosition = firstInstruction;
            while(currentPosition != null)
            {
                currentPosition.DumpToConsole();
                currentPosition = currentPosition.nextInstruction;
            }
        }

        public override string ToString()
        {
            if(name == null)
                return "%" + index.ToString();
            return name + "%" + index.ToString();
        }
    }
}
