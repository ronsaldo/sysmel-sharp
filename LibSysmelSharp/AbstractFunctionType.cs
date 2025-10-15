namespace LibSysmelSharp
{
    public class AbstractFunctionType : SysmelType
    {
        public AbstractFunctionType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsFunctionType()
        {
            return true;
        }
    }
}
