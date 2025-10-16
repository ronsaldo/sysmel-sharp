using System.Xml.Linq;

namespace LibSysmelSharp
{
    public class PointerLikeType : DerivedType
    {
        public PointerLikeType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition, baseType)
        {
        }

        public override bool IsPointerLikeType()
        {
            return true;
        }


    }
}
