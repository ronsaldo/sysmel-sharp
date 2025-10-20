namespace LibSysmelSharp
{
    public class HIRConstant : HIRValue
    {
        public HIRConstant(SourcePosition? sourcePosition, SysmelType type) : base(sourcePosition, type)
        {
        }

        public override bool IsConstant()
        {
            return true;
        }
    }
}
