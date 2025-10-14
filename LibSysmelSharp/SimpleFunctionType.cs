namespace LibSysmelSharp
{
    public class SimpleFunctionType : AbstractFunctionType
    {
        public SysmelType[] argumentTypes;
        public SysmelType resultType;

        public SimpleFunctionType(SourcePosition? sourcePosition, SysmelType[] argumentTypes, SysmelType resultType) : base(sourcePosition)
        {
            this.argumentTypes = argumentTypes;
            this.resultType = resultType;
        }


    }
}
