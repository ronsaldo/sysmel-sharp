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

        public virtual Value AnalyzeAndEvaluateInEnvironmentWithDecayedType(Environment environment)
        {
            // TODO: Load references.
            return AnalyzeAndEvaluateInEnvironment(environment);
        }

        public virtual Value AnalyzeAndEvaluateInEnvironmentWithExpectedType(Environment environment, SysmelType? expectedType)
        {
            Value evalResult = AnalyzeAndEvaluateInEnvironment(environment);
            if (expectedType == null)
                return evalResult;
            SysmelType valueType = evalResult.GetSysmelType();
            if (!expectedType.IsSatisfiedBy(valueType))
                throw new SysmelException(sourcePosition, "Expected an expression with type " + expectedType.ToString() + " instead of " + valueType.ToString());
            return evalResult;
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
            Value? method = LookupSelector(selector);
            if(method != null)
            {
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

        public virtual Value AnalyzeAndEvaluateApplicationInEnvironmentWithFunctional(ParseTreeApplicationNode application, Environment environment, Value functional)
        {
            throw new SysmelException(application.sourcePosition, "Non functional value.");
        }

        public virtual Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
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

        public virtual Value? LookupSelector(string selector)
        {
            return GetSysmelType().LookupSelector(selector);
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
            throw new NotSupportedException();
        }
        public virtual string AsSymbol()
        {
            throw new NotSupportedException();
        }

        public virtual bool IsAssociationValue()
        {
            return false;
        }

        public virtual bool IsImmutableDictionaryValue()
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
        public virtual bool IsTupleValue()
        {
            return false;
        }

        public virtual bool IsMacroFunction()
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

    }
}
