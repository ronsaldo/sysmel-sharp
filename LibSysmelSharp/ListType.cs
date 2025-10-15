namespace LibSysmelSharp
{
    public class ListType : DerivedType
    {
        public ListType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition, baseType)
        {
        }

        public override bool IsListType()
        {
            return true;
        }
    }
}
