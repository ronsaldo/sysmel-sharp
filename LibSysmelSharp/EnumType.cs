namespace LibSysmelSharp
{
    public class EnumType : SysmelType
    {
        public string? name;
        public SysmelType baseType;

        public Dictionary<string, Value> nameAndValues = new Dictionary<string, Value> ();
        public List<Value> values = new List<Value>();
        public Dictionary<string, Value>? methodDictionary;

        public EnumType(SourcePosition? sourcePosition, string? name, SysmelType baseType) : base(sourcePosition)
        {
            this.name = name;
            this.baseType = baseType;
        }

        public override bool IsEnumType()
        {
            return true;
        }

        public override string? GetName()
        {
            return name;
        }
        public override string ToString()
        {
            if (name == null)
                return "An Enum";
            return name;
        }

        public override Value? LookupSelector(string selector)
        {
            Dictionary<string, Value> methodDict = ValidMethodDictionary();
            if (methodDict.ContainsKey(selector))
                return methodDict[selector];

            return base.LookupSelector(selector);
        }

        public override Value PerformWithArguments(string selector, Value[] arguments)
        {
            if(nameAndValues.ContainsKey(selector))
                return nameAndValues[selector];
            return base.PerformWithArguments(selector, arguments);
        }

        protected Dictionary<string, Value> ValidMethodDictionary()
        {
            if (methodDictionary != null)
                return methodDictionary;

            methodDictionary = new Dictionary<string, Value> ();

            SysmelType integerType = Intrinsics.GetSingleton().IntegerType;
            SysmelType booleanType = Intrinsics.GetSingleton().BooleanType;

            AddMethod(new PrimitiveFunction("value", false, true,
                new SimpleFunctionType(null, [this], baseType),
                (Value[] arguments) => {
                    EnumValue enumValue = (EnumValue)arguments[0];
                    return enumValue.value;
                }));
            AddMethod(new PrimitiveFunction("hash", false, true,
                new SimpleFunctionType(null, [this], integerType),
                (Value[] arguments) => {
                    return new IntegerValue(null, arguments[0].GetHashCode(), integerType);
                }));
            AddMethod(new PrimitiveFunction("=", false, true,
                new SimpleFunctionType(null, [this, this], booleanType),
                (Value[] arguments) => {
                    return BooleanValue.GetWithValue(arguments[0].Equals(arguments[1]));
                }));
            AddMethod(new PrimitiveFunction("~=", false, true,
                new SimpleFunctionType(null, [this, this], booleanType),
                (Value[] arguments) => {
                    return BooleanValue.GetWithValue(!arguments[0].Equals(arguments[1]));
                }));


            return methodDictionary;
        }

        public override void WithSelectorAddMethod(string selector, Value method)
        {
            ValidMethodDictionary()[selector] = method;
        }
    }
}
