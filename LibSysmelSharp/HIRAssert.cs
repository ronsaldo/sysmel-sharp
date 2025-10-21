namespace LibSysmelSharp
{
    public class HIRAssert : HIRInstruction
    {
        public HIRValue condition;

        public HIRAssert(SourcePosition? sourcePosition, HIRValue condition)
            : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.condition = condition;
        }

        public override bool IsAssertInstruction()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  assert: " + condition.ToString());
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value conditionValue = condition.GetValueInEvaluationContext(context);
            if(!conditionValue.AsBoolean())
            {
                if (sourcePosition != null)
                    throw new SysmelException(sourcePosition, "Assertion Failure: " + sourcePosition.GetText());
                else
                    throw new SysmelException(sourcePosition, "Assertion Failure");
            }
        }
    }
}
