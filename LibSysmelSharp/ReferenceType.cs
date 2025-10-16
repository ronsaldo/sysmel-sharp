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

        public override bool Equals(object? obj)
        {
            if (obj is not ReferenceType)
                return false;

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
