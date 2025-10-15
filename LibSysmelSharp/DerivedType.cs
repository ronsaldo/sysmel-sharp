namespace LibSysmelSharp
{
    public class DerivedType : SysmelType
    {
        public SysmelType baseType;

        public DerivedType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition)
        {
            this.baseType = baseType;
        }

        public override SysmelType GetBaseType()
        {
            return baseType;
        }

    }
}
