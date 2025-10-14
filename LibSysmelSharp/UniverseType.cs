using System.Xml.Linq;

namespace LibSysmelSharp
{
    public class UniverseType : SimpleType
    {
        public int index;
        private UniverseType? nextUniverseType;

        public UniverseType(SourcePosition? sourcePosition, int index) : base(sourcePosition)
        {
            this.index = index;
        }

        public override string? GetName()
        {
            if (index == 0)
                return "Type";
            return "Type@" + index.ToString();
        }

        public override SysmelType GetSysmelType()
        {
            if (nextUniverseType == null)
                nextUniverseType = new UniverseType(sourcePosition, index + 1);
            return nextUniverseType;
        }
    }
}
