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

        public override bool Equals(object? obj)
        {
            if (obj is not ListType)
                return false;

            return base.Equals(obj);
        }
    }
}
