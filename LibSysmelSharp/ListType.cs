namespace LibSysmelSharp
{
    public class ListType : DerivedType
    {
        public Dictionary<string, Value>? methodDictionary;

        public ListType(SourcePosition? sourcePosition, SysmelType baseType) : base(sourcePosition, baseType)
        {
        }

        public override bool IsListType()
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ListType)
                return false;

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Value MakeDefaultValue()
        {
            return NilValue.GetSingleton();
        }

        public override string ToString()
        {
            return baseType.ToString() + " list";
        }

        public override Value BasicNewWithSize(long size)
        {
            Value[] listValues = new Value[size];
            Value defaultBaseTypeValue = baseType.MakeDefaultValue();
            for(long i = 0; i < size; ++i)
                listValues[i] = defaultBaseTypeValue;
            return new ListValue(sourcePosition, listValues, this);
        }

        public override Value? LookupSelector(string selector)
        {
            Dictionary<string, Value> methodDict = ValidMethodDictionary();
            if (methodDict.ContainsKey(selector))
                return methodDict[selector];

            return base.LookupSelector(selector);
        }

        protected Dictionary<string, Value> ValidMethodDictionary()
        {
            if (methodDictionary != null)
                return methodDictionary;

            SysmelType integerType = Intrinsics.GetSingleton().IntegerType;

            methodDictionary = new Dictionary<string, Value>();

            AddMethod(new PrimitiveFunction("size", false, true,
                new SimpleFunctionType(null, [this], integerType),
                (Value[] arguments) => {
                    ListValue list = (ListValue)arguments[0];
                    return new IntegerValue(null, list.elements.Length, integerType);
                }));
            AddMethod(new PrimitiveFunction("basicAt:", false, false,
                new SimpleFunctionType(null, [this, integerType], baseType),
                (Value[] arguments) => {
                    ListValue list = (ListValue)arguments[0];
                    long index = arguments[1].AsInteger();
                    return list.elements[index];
                }));
            AddMethod(new PrimitiveFunction("basicAt:put:", false, false,
                new SimpleFunctionType(null, [this, integerType, baseType], baseType),
                (Value[] arguments) => {
                    ListValue list = (ListValue)arguments[0];
                    long index = arguments[1].AsInteger();
                    Value valueToStore = arguments[2];

                    list.elements[index] = valueToStore;
                    return list;
                }));
            AddMethod(new PrimitiveFunction("at:", false, false,
                new SimpleFunctionType(null, [this, integerType], baseType),
                (Value[] arguments) => {
                    ListValue list = (ListValue)arguments[0];
                    long index = arguments[1].AsInteger();
                    return list.elements[index];
                }));
            AddMethod(new PrimitiveFunction("at:put:", false, false,
                new SimpleFunctionType(null, [this, integerType, baseType], baseType),
                (Value[] arguments) => {
                    ListValue list = (ListValue)arguments[0];
                    long index = arguments[1].AsInteger();
                    Value valueToStore = arguments[2];

                    list.elements[index] = valueToStore;
                    return list;
                }));

            return methodDictionary;
        }
        public override void WithSelectorAddMethod(string selector, Value method)
        {
            ValidMethodDictionary()[selector] = method;
        }
    }
}
