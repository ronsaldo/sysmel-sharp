namespace LibSysmelSharp
{
    public class PointerType: PointerLikeType
    {
        public PointerType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition, baseType)
        {
        }

        public override bool IsPointerType()
        {
            return true;
        }
    }
}
