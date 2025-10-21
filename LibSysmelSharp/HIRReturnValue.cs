namespace LibSysmelSharp
{
    public class HIRReturnValue: HIRInstruction
    {
        public HIRValue valueToReturn;

        public HIRReturnValue(SourcePosition? sourcePosition, HIRValue valueToReturn) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.valueToReturn = valueToReturn;
        }
        public override bool IsReturnValue()
        {
            return true;
        }

        public override bool IsTerminator()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  return: " + valueToReturn.ToString());
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value returnValue = valueToReturn.GetValueInEvaluationContext(context);
            context.returnValue = returnValue;
        }
    }
}
