using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRSwitch : HIRInstruction
    {
        public HIRValue switchValue;
        public HIRValue[] caseValues;
        public HIRBasicBlock[] caseBlocks;
        public HIRBasicBlock defaultDestination;

        public HIRSwitch(SourcePosition? sourcePosition, HIRValue switchValue, HIRValue[] caseValues, HIRBasicBlock[] caseBlocks, HIRBasicBlock defaultDestination) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            Debug.Assert(caseValues.Length == caseBlocks.Length);
            this.switchValue = switchValue;
            this.caseValues = caseValues;
            this.caseBlocks = caseBlocks;
            this.defaultDestination = defaultDestination;
        }
        public override bool IsSwitchInstruction()
        {
            return false;
        }

        public override bool IsTerminator()
        {
            return true;
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value selectValue = switchValue.GetValueInEvaluationContext(context);
            int caseCount = caseValues.Length;
            for(int caseIndex = 0; caseIndex < caseCount; caseIndex++)
            {
                Value caseValue = caseValues[caseIndex].GetValueInEvaluationContext(context);
                if(caseValue.Equals(selectValue))
                {
                    HIRBasicBlock caseBlock = caseBlocks[caseIndex];
                    context.pc = caseBlock.index;
                    return;
                }
            }

            context.pc = defaultDestination.index;
        }

        public override void DumpToConsole()
        {
            Console.Write("  switch " + switchValue.ToString() + " cases ");
            for (int i = 0; i < caseValues.Length; ++i)
            {
                if (i > 0)
                    Console.Write(", ");
                Console.Write("[" + caseValues[i].ToString() + " -> " + caseBlocks[i].ToString() + "]");
            }

            Console.WriteLine(" default " + defaultDestination.ToString());
        }
    }
}
