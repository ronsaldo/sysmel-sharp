namespace LibSysmelSharp
{
    public class HIRBoxDynamic : HIRInstruction
    {
        public HIRValue valueToBox;

        public HIRBoxDynamic(SourcePosition? sourcePosition, HIRValue valueToBox) : base(sourcePosition, Intrinsics.GetSingleton().DynamicType, null)
        {
            this.valueToBox = valueToBox;
        }

        public override bool IsBoxDynamic()
        {
            return true;
        }
    }
}
