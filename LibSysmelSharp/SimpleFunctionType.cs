using System.Text;
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

        public override bool IsSimpleFunctionType()
        {
            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            for(int i = 0; i < argumentTypes.Length; ++i)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(argumentTypes[i].ToString());
            }
            builder.Append(") => ");
            builder.Append(resultType.ToString());
            return builder.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SimpleFunctionType)
                return false;

            SimpleFunctionType other = (SimpleFunctionType)obj;
            return argumentTypes.SequenceEqual(other.argumentTypes) && resultType.Equals(other.resultType);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach(SysmelType arg in argumentTypes)
                hashCode ^= arg.GetHashCode();
            return hashCode ^ resultType.GetHashCode();
        }

        public Value ExpandFunctionalMacro(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            int argumentCount = application.arguments.Length;
            if (argumentCount + 1 != argumentTypes.Length)
                throw new SysmelException(application.sourcePosition, "Macro expects" + argumentTypes.Length.ToString() + " arguments instead of" + application.arguments.Length.ToString());


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

            int applicationArgumentCount = application.arguments.Length;
            if (applicationArgumentCount != argumentTypes.Length)
                throw new SysmelException(application.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + application.arguments.Length.ToString());

            Value[] callArguments = new Value[argumentTypes.Length];
            for (int i = 0; i < applicationArgumentCount; ++i)
                callArguments[i] = application.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[i]);

            return functional.EvaluateWithArguments(callArguments);
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
