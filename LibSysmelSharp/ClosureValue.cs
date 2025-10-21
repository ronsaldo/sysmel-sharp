namespace LibSysmelSharp
{
    public class ClosureValue : AbstractFunction
    {
        public Value functionValue;
        public Value[] captureVector;
        public ClosureValue(SourcePosition? sourcePosition, string? name, bool isMacro, bool isCompileTime, SysmelType functionType, Value functionValue, Value[] captureVector)
            : base(sourcePosition, name, isMacro, isCompileTime, functionType)
        {
            this.functionValue = functionValue;
            this.captureVector = captureVector;
        }

        public override bool IsClosureValue()
        {
            return true;
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            return functionValue.EvaluateWithArgumentsAndCaptures(callArguments, captureVector);
        }
    }
}
