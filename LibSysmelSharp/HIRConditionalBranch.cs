namespace LibSysmelSharp
{
    public class HIRConditionalBranch : HIRInstruction
    {
        public HIRValue condition;
        public HIRBasicBlock trueDestination;
        public HIRBasicBlock falseDestination;

        public HIRConditionalBranch(SourcePosition? sourcePosition, HIRValue condition, HIRBasicBlock trueDestination, HIRBasicBlock falseDestination) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.condition = condition;
            this.trueDestination = trueDestination;
            this.falseDestination = falseDestination;
        }

        public override bool IsConditionalBranchInstruction()
        {
            return true;
        }

        public override bool IsTerminator()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  conditionalBranch " + condition.ToString() + " " + trueDestination.ToString() + " " + falseDestination.ToString());
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value conditionValue = condition.GetValueInEvaluationContext(context);
            if(conditionValue.AsBoolean())
                context.pc = trueDestination.index;
            else
                context.pc = falseDestination.index;
        }
    }
}
