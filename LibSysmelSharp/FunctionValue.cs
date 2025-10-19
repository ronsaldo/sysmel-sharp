using System.Diagnostics;

namespace LibSysmelSharp
{
    public class FunctionValue : Value
    {
        public Value? owner;
        public string? name;
        public DependentFunctionType signature;
        public Environment definitionEnvironment;
        public ParseTreeNode definitionBody;
        public SysmelType type;
        public bool isTemplate;
        public Dictionary<MemoizedFunctionKey, TemplateInstance> memoizedValues;
        public List<TemplateInstance> instances;
        public List<TemplateExtensionMethod> extensions;

        public FunctionValue(SourcePosition? sourcePosition, string? name, DependentFunctionType signature, Environment definitionEnvironment, ParseTreeNode definitionBody) : base(sourcePosition)
        {
            this.name = name;
            this.signature = signature;
            this.definitionEnvironment = definitionEnvironment;
            this.definitionBody = definitionBody;
            this.type = signature.TryToSimplifyFunctionType();
            this.isTemplate = signature.isForAll;
            this.memoizedValues = new Dictionary<MemoizedFunctionKey, TemplateInstance>();
            this.instances = new List<TemplateInstance>();
            this.extensions = new List<TemplateExtensionMethod>();
        }

        public override bool IsFunctionValue()
        {
            return true;
        }
        public override bool IsTemplate()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string? GetName()
        {
            return name;
        }

        public override void SetOwner(Value anOwner)
        {
            this.owner = anOwner;
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            MemoizedFunctionKey? memoizationKey = null;
            if (isTemplate)
            {
                memoizationKey = new MemoizedFunctionKey(callArguments);
                if (memoizedValues.ContainsKey(memoizationKey))
                    return memoizedValues[memoizationKey].instanceValue;
            }

            FunctionActivationEnvironment activationEnvironment = new FunctionActivationEnvironment(definitionEnvironment);
            foreach(ArgumentDefinitionValue argumentDef in signature.arguments)
            {
                if(argumentDef.name != null)
                {
                    Value argumentValue = callArguments[argumentDef.index];
                    activationEnvironment.SetSymbolBinding(argumentDef.name, argumentValue);
                }
            }

            Value resultType = signature.resultTypeExpression.AnalyzeAndEvaluateInEnvironment(activationEnvironment);
            if (!resultType.IsType())
                throw new SysmelException(signature.resultTypeExpression.sourcePosition, "Expected a result type expression.");

            SysmelType resultTypeValue = (SysmelType)resultType;

            Value resultValue = VoidValue.GetSingleton();
            LexicalEnvironment bodyEnvironment = new LexicalEnvironment(activationEnvironment);
            try
            {
                resultValue = definitionBody.AnalyzeAndEvaluateInEnvironmentWithExpectedType(bodyEnvironment, resultTypeValue);
            }
            catch (SysmelReturnException re)
            {
                resultValue = re.resultValue.AnalyzeAndEvaluateInEnvironmentWithExpectedType(bodyEnvironment, resultTypeValue);
            }

            if (isTemplate && memoizationKey != null)
            {
                TemplateInstance instance = new TemplateInstance(memoizationKey, resultValue);
                resultValue.SetOwner(this);
                memoizedValues[memoizationKey] = instance;
                instances.Add(instance);

                ApplyExtensionsToTemplateInstance(instance);
            }

            return resultValue;
        }

        public override Value AddExtensionMethodWithEnvironment(ParseTreeMethodNode methodNode, Environment environment)
        {
            TemplateExtensionMethod extensionMethod = new TemplateExtensionMethod(methodNode, environment);
            AddNewExtension(extensionMethod);
            return this;
        }

        void ApplyExtensionToTemplateInstance(TemplateInstance instanceValue, TemplateExtensionMethod extensionMethod)
        {
            FunctionActivationEnvironment activationEnvironment = new FunctionActivationEnvironment(extensionMethod.environment);
            foreach (ArgumentDefinitionValue argumentDef in signature.arguments)
            {
                if (argumentDef.name != null)
                {
                    Value argumentValue = instanceValue.instanceArguments.arguments[argumentDef.index];
                    activationEnvironment.SetSymbolBinding(argumentDef.name, argumentValue);
                }
            }

            LexicalEnvironment extensionBodyEnvironment = new LexicalEnvironment(activationEnvironment);
            extensionMethod.methodNode.AnalyzeAndEvaluateInEnvironmentAddingToOwner(extensionBodyEnvironment, instanceValue.instanceValue);
        }

        void ApplyExtensionsToTemplateInstance(TemplateInstance instanceValue)
        {
            foreach (TemplateExtensionMethod extension in extensions)
                ApplyExtensionToTemplateInstance(instanceValue, extension);
        }

        public void AddNewExtension(TemplateExtensionMethod extensionValue)
        {
            if (!isTemplate)
                throw new SysmelException(extensionValue.methodNode.sourcePosition, "Cannot add template extensions method to non-template " + ToString());
            extensions.Add(extensionValue);
            foreach (TemplateInstance instance in instances)
                ApplyExtensionToTemplateInstance(instance, extensionValue);
        }

    }
}
