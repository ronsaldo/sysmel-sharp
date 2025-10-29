using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace LibSysmelSharp
{
    public class Value
    {
        public SourcePosition? sourcePosition;

        public Value(SourcePosition? sourcePosition)
        {
            this.sourcePosition = sourcePosition;
        }

        public virtual Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return this;
        }

        public virtual Value AnalyzeAndEvaluateInEnvironmentWithEnumBaseType(Environment environment, SysmelType baseType)
        {
            return AnalyzeAndEvaluateInEnvironment(environment);

        }
        public virtual Value AnalyzeAndEvaluateInEnvironmentWithDecayedType(Environment environment)
        {
            Value evaluatedValue = AnalyzeAndEvaluateInEnvironment(environment);
            SysmelType valueType = evaluatedValue.GetSysmelType();
            if (valueType.IsReferenceType())
                return evaluatedValue.LoadValue();

            return evaluatedValue;
        }

        public virtual Value CastAnalyzedAndEvaluatedValueToExpectedType(Value evaluationResult, SysmelType? expectedType)
        {
            SysmelType valueType = evaluationResult.GetSysmelType();
            if (valueType.IsControlFlowEscapeType())
                return evaluationResult;

            if (expectedType == null)
            {
                if (valueType.IsReferenceType())
                    return evaluationResult.LoadValue();

                return evaluationResult;
            }
            if (expectedType.IsVoidType())
                return VoidValue.GetSingleton();
            
            if (expectedType.IsSatisfiedBy(valueType))
                return evaluationResult;

            if(valueType.IsReferenceType())
                return CastAnalyzedAndEvaluatedValueToExpectedType(evaluationResult.LoadValue(), expectedType);

            if(expectedType == Intrinsics.GetSingleton().DynamicType)
                return evaluationResult;
            throw new SysmelException(sourcePosition, "Expected an expression with type " + expectedType.ToString() + " instead of " + valueType.ToString());
        }

        public virtual Value AnalyzeAndEvaluateInEnvironmentWithExpectedType(Environment environment, SysmelType? expectedType)
        {
            Value evalResult = AnalyzeAndEvaluateInEnvironment(environment);
            return CastAnalyzedAndEvaluatedValueToExpectedType(evalResult, expectedType);
        }
        public virtual Value AnalyzeAndEvaluateMacroArgumentInEnvironment(Environment environment, SysmelType expectedType)
        {
            return AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, expectedType);
        }

        public virtual Value AnalyzeAndEvaluateIdentifierReferenceInEnvironment(ParseTreeIdentifierReferenceNode identifierNode, Environment environment)
        {
            return this;
        }

        public virtual Value AnalyzeAndEvaluateMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            // HACK: Use a macro or something else here.
            if (selector == "yourself")
                return this;

            Value? method = LookupSelector(selector);
            if(method != null)
            {
                if(method.IsFieldDefinition())
                    return method.AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(messageSend, selector, environment, this, method);
                SysmelType functionType = method.GetSysmelType();
                return functionType.AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(messageSend, selector, environment, this, method);

            }

            Value[] argumentValues = new Value[messageSend.arguments.Length];
            for (int i = 0; i < messageSend.arguments.Length; ++i)
                argumentValues[i] = messageSend.arguments[i].AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            return PerformWithArguments(selector, argumentValues);
        }

        public virtual Value AnalyzeAndEvaluateApplicationInEnvironment(ParseTreeApplicationNode application, Environment environment)
        {
            return GetSysmelType().AnalyzeAndEvaluateApplicationInEnvironmentWithFunctional(application, environment, this);
        }
        public virtual HIRValue AnalyzeAndBuildApplication(ParseTreeApplicationNode application, HIRBuilder builder)
        {
            return GetSysmelType().AnalyzeAndBuildApplicationInEnvironmentWithFunctional(application, builder, AnalyzeAndBuild(builder));
        }

        public virtual Value AnalyzeAndEvaluateApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            throw new SysmelException(application.sourcePosition, "Non functional value.");
        }
        public virtual HIRValue AnalyzeAndBuildApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, HIRBuilder builder, HIRValue functional)
        {
            throw new SysmelException(application.sourcePosition, "Non functional value.");
        }

        public virtual Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            throw new SysmelException(messageSend.sourcePosition, "Found method does not have a functional type.");
        }

        public virtual HIRValue AnalyzeAndBuildMessageSendWithMethod(ParseTreeMessageSendNode messageSend, string selector, HIRBuilder builder, HIRValue receiver, Value method)
        {
            throw new SysmelException(messageSend.sourcePosition, "Found method does not have a functional type.");
        }

        public virtual Value AnalyzeAndEvaluateAssignmentInEnvironment(ParseTreeAssignmentNode assignmentNode, Environment environment)
        {
            SysmelType storageType = GetSysmelType();
            if(!storageType.IsReferenceType())
                throw new SysmelException(assignmentNode.sourcePosition, "Storage does not support assignments.");
            
            SysmelType baseType = storageType.GetBaseType();
            Value valueToStore = assignmentNode.value.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, baseType);
            this.StoreValue(valueToStore);
            return this;
        }

        public virtual HIRValue AnalyzeAndBuildAssignment(ParseTreeAssignmentNode assignmentNode, HIRBuilder builder)
        {
            SysmelType storageType = GetSysmelType();
            if (!storageType.IsReferenceType())
                throw new SysmelException(assignmentNode.sourcePosition, "Storage does not support assignments.");

            SysmelType baseType = storageType.GetBaseType();
            HIRValue valueToStore = assignmentNode.value.AnalyzeAndBuildWithExpectedType(builder, baseType);
            HIRValue storagePointer = AnalyzeAndBuild(builder);
            builder.StorePointer(assignmentNode.sourcePosition, storagePointer, valueToStore);
            return storagePointer;

        }
        public virtual ParseTreeArgumentDefinitionNode ParseAsArgumentDefinitionInEnvironment(Environment environment)
        {
            throw new SysmelException(sourcePosition, "Invalid argument spec.");
        }
        public virtual ParseTreeArgumentDefinitionNode[] ParseAsArgumentDefinitionsInEnvironment(Environment environment)
        {
            throw new SysmelException(sourcePosition, "Invalid arguments spec.");
        }

        public virtual Value AnalyzeAndEvaluateDictionaryInstanceInEnvironment(ParseTreeImmutableDictionaryNode dictionary, Environment environment)
        {
            throw new SysmelException(sourcePosition, "No dictionary type expression.");
        }

        public virtual HIRValue AnalyzeAndBuildDictionaryInstance(ParseTreeImmutableDictionaryNode dictionary, HIRBuilder builder)
        {
            throw new SysmelException(sourcePosition, "No dictionary type expression.");
        }

        public virtual HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return builder.AddLiteral(sourcePosition, this);
        }

        public virtual HIRValue AnalyzeAndBuildWithDecayedType(HIRBuilder builder)
        {
            HIRValue irvalue = AnalyzeAndBuild(builder);
            SysmelType valueType = irvalue.GetSysmelType();
            if (valueType.IsReferenceType())
                return builder.LoadPointer(sourcePosition, irvalue);

            return irvalue;
        }

        public virtual HIRValue AnalyzeAndBuildWithExpectedType(HIRBuilder builder, SysmelType? expectedType)
        {
            HIRValue value = AnalyzeAndBuild(builder);
            return CastHIRValueToExpectedType(builder, value, expectedType);
        }

        public virtual HIRValue AnalyzeAndBuildIdentifierReference(ParseTreeIdentifierReferenceNode node, HIRBuilder builder)
        {
            return builder.AddLiteral(node.sourcePosition, this);
        }
        public virtual HIRValue AnalyzeAndBuildMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, HIRBuilder builder)
        {
            // HACK: Use a macro or something else here.
            HIRValue receiver = this.AnalyzeAndBuild(builder);
            if (selector == "yourself")
                return receiver;

            if (receiver.GetSysmelType().IsReferenceType())
                receiver = builder.LoadPointer(sourcePosition, receiver);


            Value? method = receiver.LookupSelector(selector);
            if (method != null)
            {
                if (method.IsFieldDefinition())
                    return method.AnalyzeAndBuildMessageSendWithMethod(messageSend, selector, builder, receiver, method);
                
                SysmelType functionType = method.GetSysmelType();
                return functionType.AnalyzeAndBuildMessageSendWithMethod(messageSend, selector, builder, receiver, method);
            }

            if(receiver.GetSysmelType().IsDynamicType())
            {
                // TODO: Emit a dynamic message send.
                throw new NotImplementedException();
            }

            throw new SysmelException(messageSend.sourcePosition, "Method with selector #" + selector + " not found in " + receiver.GetSysmelType().ToString());
        }

        public virtual HIRValue CastHIRValueToExpectedType(HIRBuilder builder, HIRValue buildResult, SysmelType? expectedType)
        {
            SysmelType valueType = buildResult.GetSysmelType();
            if (valueType.IsControlFlowEscapeType())
                return buildResult;
            if (expectedType == null)
            {
                if (valueType.IsReferenceType())
                    return builder.LoadPointer(sourcePosition, buildResult);
                return buildResult;
            }
                
            if (expectedType.IsVoidType())
                return builder.GetVoidLiteral(sourcePosition);

            if (expectedType.IsSatisfiedBy(valueType))
                return buildResult;

            if (valueType.IsReferenceType())
            {
                HIRValue loadedValue = builder.LoadPointer(sourcePosition, buildResult);
                return CastHIRValueToExpectedType(builder, loadedValue, expectedType);
            }

            SysmelType DynamicType = Intrinsics.GetSingleton().DynamicType;
            if (expectedType == DynamicType)
                return builder.BoxDynamic(sourcePosition, buildResult);
            else if(valueType == DynamicType)
                return builder.UnboxDynamic(sourcePosition, buildResult, expectedType);

            throw new SysmelException(sourcePosition, "Expected an expression with type " + expectedType.ToString() + " instead of " + valueType.ToString());
        }


        public virtual void EnsureAnalysis()
        {
        }

        public void EnqueuePendingAnalysis()
        {
            Intrinsics.GetSingleton().AddValueWithPendingAnalysis(this);
        }

        public virtual Value? LookupSelector(string selector)
        {
            return GetSysmelType().LookupSelector(selector);
        }

        public virtual FieldDefinition? LookupFieldName(string selector)
        {
            return GetSysmelType().LookupFieldName(selector);
        }

        public virtual void WithSelectorAddMethod(string selector, Value method)
        {
            GetSysmelType().WithSelectorAddMethod(selector, method);
        }

        public void AddMethod(AbstractFunction method)
        {
            Debug.Assert(method.name != null);
            WithSelectorAddMethod(method.name, method);
        }

        public virtual string? GetName()
        {
            return null;
        }

        public virtual void SetOwner(Value anOwner)
        {
            throw new NotSupportedException();
        }
        public virtual void SetSymbolBinding(string symbol, Value binding)
        {
            throw new NotSupportedException();
        }
        
        public virtual Value AddExtensionMethodWithEnvironment(ParseTreeMethodNode methodNode, Environment environment)
        {
            throw new NotSupportedException();
        }

        public virtual Value PerformWithArguments(string selector, Value[] arguments)
        {
            Value? foundMethod = LookupSelector(selector);
            if (foundMethod == null)
                throw new SysmelException(sourcePosition, "Message not understood #" + selector);

            Value[] receiverArgument = new Value[] { this };
            Value[] callArguments = receiverArgument.Concat(arguments).ToArray();
            return foundMethod.EvaluateWithArguments(callArguments);
        }
        public virtual Value EvaluateWithArguments(Value[] callArguments)
        {
            throw new SysmelException(sourcePosition, "Cannot evaluate non-functional value.");
        }

        public virtual Value EvaluateWithArgumentsAndCaptures(Value[] callArguments, Value[] callCaptures)
        {
            throw new SysmelException(sourcePosition, "Cannot evaluate non-functional value.");
        }


        public virtual bool AsBoolean()
        {
            throw new NotSupportedException();
        }

        public virtual long AsInteger()
        {
            throw new NotSupportedException();
        }
        public virtual double AsFloat()
        {
            throw new NotSupportedException();
        }

        public virtual string AsString()
        {
            string ?str = ToString();
            if (str != null)
                return str;
            return "a Value";
        }
        public virtual string AsSymbol()
        {
            throw new NotSupportedException();
        }

        public virtual bool IsHIRValue()
        {
            return false;
        }

        public virtual bool IsAssociationValue()
        {
            return false;
        }
        public virtual bool IsImplicitFieldAccess()
        {
            return false;
        }

        public virtual bool IsImmutableDictionaryValue()
        {
            return false;
        }

        public virtual bool IsMetaBuilder()
        {
            return false;
        }

        public virtual bool IsMetaBuilderFactory()
        {
            return false;
        }

        public virtual bool IsParseTreeNode()
        {
            return false;
        }

        public virtual bool IsType()
        {
            return false;
        }
        public virtual bool IsVoidValue()
        {
            return false;
        }

        public virtual bool IsControlFlowEscapeValue()
        {
            return false;

        }
        public virtual bool IsNilValue()
        {
            return false;
        }

        public virtual bool IsBooleanValue()
        {
            return false;
        }

        public virtual bool IsIntegerValue()
        {
            return false;
        }

        public virtual bool IsFloatValue()
        {
            return false;
        }

        public virtual bool IsStringValue()
        {
            return false;
        }

        public virtual bool IsSymbolValue()
        {
            return false;
        }
        public virtual bool isMacroContextValue()
        {
            return false;
        }
        public virtual bool IsListValue()
        {
            return false;
        }

        public virtual bool IsByteArrayValue()
        {
            return false;
        }

        public virtual bool IsTupleValue()
        {
            return false;
        }

        public virtual bool IsArgumentDefinitionValue()
        {
            return false;
        }

        public virtual bool IsAbstractFunctionValue()
        {
            return false;
        }
        public virtual bool IsPrimitiveFunctionValue()
        {
            return false;
        }
        public virtual bool IsFunctionValue()
        {
            return false;
        }
        public virtual bool IsClosureValue()
        {
            return false;
        }
        public virtual bool IsTemplate()
        {
            return false;
        }
        public virtual bool IsMacroFunction()
        {
            return false;
        }
        public virtual bool IsCompileTimeFunction()
        {
            return false;
        }
        public virtual bool IsPointerLikeValue()
        {
            return false;
        }
        public virtual bool IsPointerValue()
        {
            return false;
        }

        public virtual bool IsReferenceLikeValue()
        {
            return false;
        }

        public virtual bool IsFieldDefinition()
        {
            return false;
        }

        public virtual bool IsEnumValue()
        {
            return false;
        }

        public virtual bool IsNamespace()
        {
            return false;
        }

        public virtual bool IsRecordValue()
        {
            return false;
        }

        public virtual bool IsClassInstanceValue()
        {
            return false;
        }

        public virtual SysmelType GetSysmelType()
        {
            throw new NotImplementedException();
        }

        public virtual Value LoadValue()
        {
            throw new NotSupportedException();
        }

        public virtual Value LoadValueWithFieldIndex(int fieldIndex)
        {
            throw new NotSupportedException();
        }

        public virtual void StoreValue(Value valueToStore)
        {
            throw new NotSupportedException();
        }

        public virtual void StoreValueWithFieldIndex(Value valueToStore, int fieldIndex)
        {
            throw new NotSupportedException();
        }

        public virtual SysmelType AsSelfTypeWithDefaultType(SysmelType defaultResultType)
        {
            return defaultResultType;
        }

        public virtual bool IsCapturableValue()
        {
            return false;
        }

        public virtual void DumpToConsole()
        {
            System.Console.WriteLine(ToString());
        }
    }
}
