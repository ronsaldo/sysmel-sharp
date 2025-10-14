using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public Value ExpandFunctionalMacro(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            int argumentCount = application.arguments.Length;
            
            MacroContextValue context = new MacroContextValue(application.sourcePosition, environment);
            Value[] macroArguments = new Value[1 + argumentCount];
            macroArguments[0] = context;

            for (int i = 0; i < argumentCount; ++i)
            {
                SysmelType expectedType = argumentTypes[1 + i];
                macroArguments[i + 1] = application.arguments[i].AnalyzeAndEvaluateMacroArgumentInEnvironment(environment, expectedType);
            }

            Value macroResult = functional.EvaluateWithArguments(macroArguments);
            if (resultType == Intrinsics.GetSingleton().ParseTreeNodeType)
                return macroResult.AnalyzeAndEvaluateInEnvironment(environment);
            else
                return macroResult.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, resultType);
        }

        public Value ExpandMessageMacro(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            throw new SysmelException(messageSend.sourcePosition, "TODO: Expand message macro.");
        }

        public override Value AnalyzeAndEvaluateApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            if (functional.IsMacroFunction())
                return ExpandFunctionalMacro(application, environment, functional);

            throw new SysmelException(application.sourcePosition, "Simple function application.");
        }

        public override Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            if (method.IsMacroFunction())
                return ExpandMessageMacro(messageSend, selector, environment, receiver, method);

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
