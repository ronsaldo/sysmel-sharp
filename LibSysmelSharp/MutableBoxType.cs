namespace LibSysmelSharp
{
    public class MutableBoxType : DerivedType
    {
        public MutableBoxType(SourcePosition? sourcePosition, SysmelType baseType   ) : base(sourcePosition, baseType)
        {
        }
        public override bool IsMutableBoxType()
        {
            return true;
        }

    }
}
