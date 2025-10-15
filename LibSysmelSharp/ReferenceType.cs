namespace LibSysmelSharp
{
    public class ReferenceType : PointerLikeType
    {
        public ReferenceType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition, baseType)
        {
        }

        public override bool IsReferenceType()
        {
            return true;
        }

        public override ReferenceType Ref()
        {
            return this;
        }
    }
}
