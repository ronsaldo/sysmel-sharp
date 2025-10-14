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
        public override Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            int messageArgumentCount = messageSend.arguments.Length;
            if (messageArgumentCount + 1 != argumentTypes.Length)
                throw new SysmelException(messageSend.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + (messageSend.arguments.Length + 1).ToString());

            Value[] callArguments = new Value[argumentTypes.Length];
            callArguments[0] = receiver.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[0]);

            for(int i = 0; i < messageArgumentCount; ++i)
                callArguments[i + 1] = messageSend.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[i + 1]);

            return method.EvaluateWithArguments(callArguments);
        }
    }
}
