using System;
using System.Text;

namespace LibSysmelSharp
{
    public delegate SysmelType DependentFunctionTypeResultComputationFunction(HIRValue[] arguments, SysmelType defaultResultType);

    public class SimpleFunctionType : AbstractFunctionType
    {
        public SysmelType[] argumentTypes;
        public SysmelType resultType;
        public DependentFunctionTypeResultComputationFunction? resultTypeComputationFunction;

        public SimpleFunctionType(SourcePosition? sourcePosition, SysmelType[] argumentTypes, SysmelType resultType) : base(sourcePosition)
        {
            this.argumentTypes = argumentTypes;
            this.resultType = resultType;
        }

        public SimpleFunctionType(SourcePosition? sourcePosition, SysmelType[] argumentTypes, SysmelType resultType, DependentFunctionTypeResultComputationFunction resultTypeComputationFunction) : base(sourcePosition)
        {
            this.argumentTypes = argumentTypes;
            this.resultType = resultType;
            this.resultTypeComputationFunction = resultTypeComputationFunction;
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

        SysmelType ComputeResultTypeWithHirArguments(HIRValue[] arguments)
        {
            if (resultTypeComputationFunction != null)
                return resultTypeComputationFunction(arguments, resultType);
            return resultType;
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

            return functional.EvaluateWithArguments(macroArguments);
        }

        public Value ExpandMessageMacro(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            throw new SysmelException(messageSend.sourcePosition, "TODO: Expand message macro.");
        }

        public override Value AnalyzeAndEvaluateApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            if (functional.IsMacroFunction())
            {
                Value macroResult = ExpandFunctionalMacro(application, environment, functional);
                if (resultType == Intrinsics.GetSingleton().ParseTreeNodeType)
                    return macroResult.AnalyzeAndEvaluateInEnvironment(environment);
                else
                    return macroResult.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, resultType);
            }

            int applicationArgumentCount = application.arguments.Length;
            if (applicationArgumentCount != argumentTypes.Length)
                throw new SysmelException(application.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + application.arguments.Length.ToString());

            Value[] callArguments = new Value[argumentTypes.Length];
            for (int i = 0; i < applicationArgumentCount; ++i)
                callArguments[i] = application.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[i]);

            return functional.EvaluateWithArguments(callArguments);
        }

        public override HIRValue AnalyzeAndBuildApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, HIRBuilder builder, HIRValue functional)
        {
            if(functional.IsMacroFunction())
            {
                HIRImportedFunction importedFunction = (HIRImportedFunction)functional;
                Value macroResult = ExpandFunctionalMacro(application, builder.environment, importedFunction.importedFunction);
                if (resultType == Intrinsics.GetSingleton().ParseTreeNodeType)
                    return macroResult.AnalyzeAndBuild(builder);
                else
                    return macroResult.AnalyzeAndBuildWithExpectedType(builder, resultType);
            }

            int applicationArgumentCount = application.arguments.Length;
            if (applicationArgumentCount != argumentTypes.Length)
                throw new SysmelException(application.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + application.arguments.Length.ToString());

            HIRValue[] callArguments = new HIRValue[argumentTypes.Length];
            for (int i = 0; i < applicationArgumentCount; ++i)
                callArguments[i] = application.arguments[i].AnalyzeAndBuildWithExpectedType(builder, argumentTypes[i]);

            SysmelType computedResultType = ComputeResultTypeWithHirArguments(callArguments);
            return builder.CallFunction(application.sourcePosition, functional, callArguments, computedResultType);
        }

        public override Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            if (method.IsMacroFunction())
            {
                Value macroResult = ExpandMessageMacro(messageSend, selector, environment, receiver, method);
                if (resultType == Intrinsics.GetSingleton().ParseTreeNodeType)
                    return macroResult.AnalyzeAndEvaluateInEnvironment(environment);
                else
                    return macroResult.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, resultType);
            }

            int messageArgumentCount = messageSend.arguments.Length;
            if (messageArgumentCount + 1 != argumentTypes.Length)
                throw new SysmelException(messageSend.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + (messageSend.arguments.Length + 1).ToString());

            Value[] callArguments = new Value[argumentTypes.Length];
            callArguments[0] = receiver.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[0]);

            for(int i = 0; i < messageArgumentCount; ++i)
                callArguments[i + 1] = messageSend.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, argumentTypes[i + 1]);

            return method.EvaluateWithArguments(callArguments);
        }

        public override HIRValue AnalyzeAndBuildMessageSendWithMethod(ParseTreeMessageSendNode messageSend, string selector, HIRBuilder builder, HIRValue receiver, Value method)
        {
            if(method.IsMacroFunction())
            {
                Value macroResult =  ExpandMessageMacro(messageSend, selector, builder.environment, receiver, method);
                if (resultType == Intrinsics.GetSingleton().ParseTreeNodeType)
                    return macroResult.AnalyzeAndBuild(builder);
                else
                    return macroResult.AnalyzeAndBuildWithExpectedType(builder, resultType);
            }

            int messageArgumentCount = messageSend.arguments.Length;
            if (messageArgumentCount + 1 != argumentTypes.Length)
                throw new SysmelException(messageSend.sourcePosition, "Expected " + argumentTypes.Length.ToString() + " arguments instead of" + (messageSend.arguments.Length + 1).ToString());

            HIRValue[] callArguments = new HIRValue[argumentTypes.Length];
            callArguments[0] = receiver.AnalyzeAndBuildWithExpectedType(builder, argumentTypes[0]);
            for (int i = 0; i < messageArgumentCount; ++i)
                callArguments[i + 1] = messageSend.arguments[i].AnalyzeAndBuildWithExpectedType(builder, argumentTypes[i + 1]);

            HIRValue function = method.AnalyzeAndBuildWithDecayedType(builder);
            SysmelType computedResultType = ComputeResultTypeWithHirArguments(callArguments);
            return builder.CallFunction(messageSend.sourcePosition, function, callArguments, computedResultType);
        }
    }
}
