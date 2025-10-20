namespace LibSysmelSharp
{
    public class HIRUnboxDynamic : HIRInstruction
    {
        public HIRValue boxedValue;

        public HIRUnboxDynamic(SourcePosition? sourcePosition, HIRValue boxedValue, SysmelType unboxedType) : base(sourcePosition, unboxedType, null)
        {
            this.boxedValue = boxedValue;
        }

        public override bool IsUnboxDynamic()
        {
            return true;
        }
    }
}
