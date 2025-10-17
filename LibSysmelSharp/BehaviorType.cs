namespace LibSysmelSharp
{
    public class BehaviorType : SysmelType
    {
        public BehaviorType? superclass;
        public FieldDefinition[] fields;
        public Dictionary<string, FieldDefinition> fieldsByName;
        public FieldDefinition[] allFields;

        public BehaviorType(SourcePosition? sourcePosition, BehaviorType? superclass, FieldDefinition[] fields) : base(sourcePosition)
        {
            this.superclass = superclass;
            this.fields = fields;
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
        
    }
}
