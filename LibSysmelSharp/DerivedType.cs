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

        public override bool Equals(object? obj)
        {
            if (obj is not DerivedType)
                return false;

            DerivedType other = (DerivedType)obj;
            return baseType.Equals(other.baseType);
        }
        public override int GetHashCode()
        {
            return baseType.GetHashCode();
        }
    }
}
