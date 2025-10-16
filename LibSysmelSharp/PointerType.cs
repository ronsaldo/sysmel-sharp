using System.Xml.Linq;

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
        public override bool Equals(object? obj)
        {
            if (obj is not PointerType)
                return false;

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
