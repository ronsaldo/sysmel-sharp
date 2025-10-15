using System.Diagnostics;

namespace LibSysmelSharp
{
    public class ReferenceLikeValue : PointerLikeValue
    {
        public ReferenceLikeValue(SourcePosition? sourcePosition, Value storage, int fieldIndex, SysmelType type) : base(sourcePosition, storage, fieldIndex, type)
        {
        }

        public override bool IsReferenceLikeValue()
        {
            return true;
        }

        public override Value PerformWithArguments(string selector, Value[] arguments)
        {
            Debug.Assert(type.IsReferenceType());
            return LoadValue().PerformWithArguments(selector, arguments);
        }

    }
}
