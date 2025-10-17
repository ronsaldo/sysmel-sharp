namespace LibSysmelSharp
{
    public class BehaviorType : SysmelType
    {
        public BehaviorType? superclass;
        public FieldDefinition[] fields;
        public Dictionary<string, FieldDefinition> fieldsByName;
        public Dictionary<string, Value> methodDict;
        public List<Value> methodList;
        public FieldDefinition[] allFields;

        public BehaviorType(SourcePosition? sourcePosition, BehaviorType? superclass, FieldDefinition[] fields) : base(sourcePosition)
        {
            this.superclass = superclass;
            this.fields = fields;
            this.methodDict = new Dictionary<string, Value>();
            this.methodList = new List<Value>();
            this.allFields = fields;
            if(superclass != null)
                this.allFields = superclass.allFields.Concat(fields).ToArray();
            
            this.fieldsByName = new Dictionary<string, FieldDefinition>();
            BuildFieldsByName();
        }

        public void SetFields(FieldDefinition[] fields)
        {
            this.fields = fields;
            this.allFields = fields;
            if (superclass != null)
                this.allFields = superclass.allFields.Concat(fields).ToArray();
            this.BuildFieldsByName();
        }

        void BuildFieldsByName()
        {
            this.fieldsByName = new Dictionary<string, FieldDefinition>();
            int fieldCount = allFields.Length;
            for(int i = 0; i < fieldCount; ++i)
            {
                FieldDefinition field = allFields[i];
                field.index = i;
                fieldsByName[field.name] = field;
            }
        }

        public override bool IsBehaviorType()
        {
            return true;
        }

        public override void WithSelectorAddMethod(string selector, Value method)
        {
            methodDict[selector] = method;
            methodList.Add(method);
        }

        public override Value? LookupSelector(string selector)
        {
            if (methodDict.ContainsKey(selector))
                return methodDict[selector];

            if (superclass != null)
                return superclass.LookupSelector(selector);
            return null;
        }

        public override Value MakeDefaultValue()
        {
            return NilValue.GetSingleton();
        }

        public ClassInstanceValue BasicNew()
        {
            int fieldCount = allFields.Length;
            Value[] instanceFields = new Value[fieldCount];
            for (int i = 0; i < fieldCount; ++i)
                instanceFields[i] = allFields[i].type.MakeDefaultValue();

            return new ClassInstanceValue(sourcePosition, instanceFields, this);
        }

        public ClassInstanceValue New()
        {
            ClassInstanceValue instance = BasicNew();
            instance.PerformWithArguments("initialize", []);
            return instance;
        }

    }
}
