
namespace LibSysmelSharp
{
    public class TupleType : SimpleType
    {
        public SysmelType[] elements;

        public TupleType(SourcePosition? sourcePosition, SysmelType[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }
        public override bool IsTupleType()
        {
            return true;
        }

        public override SimpleFunctionType MakeSimpleFunctionWithResultType(SysmelType resultType)
        {
            return new SimpleFunctionType(sourcePosition, elements, resultType);
        }
    }
}
