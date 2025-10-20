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
    }
}
