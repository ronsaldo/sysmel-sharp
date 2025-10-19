using static System.Net.Mime.MediaTypeNames;

namespace LibSysmelSharp
{
    public class RecordType : SimpleType
    {
        public string? name;
        public FieldDefinition[] fields;
        public Dictionary<string, FieldDefinition> fieldsByName;

        public RecordType(SourcePosition? sourcePosition, string? name, FieldDefinition[] fields) : base(sourcePosition)
        {
            this.name = name;
            this.fields = fields;
            this.fieldsByName = new Dictionary<string, FieldDefinition>();
            BuildFieldsByName();
        }

        public void BuildFieldsByName()
        {
            this.fieldsByName = new Dictionary<string, FieldDefinition>();
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldDefinition field = this.fields[i];
                field.index = i;
                this.fieldsByName[field.name] = field;
            }
        }

        public override bool IsRecordType()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateApplicationInEnvironment(ParseTreeApplicationNode application, Environment environment)
        {
            int fieldCount = fields.Length;
            int argumentCount = application.arguments.Length;
            if (argumentCount > fields.Length)
                throw new SysmelException(application.sourcePosition, "Record expects at most " + fields.Length.ToString() + " arguments");

            Value[] recordElements = new Value[fields.Length];
            for(int i = 0; i < fieldCount; ++i)
            {
                FieldDefinition field = fields[i];

                if(i < argumentCount)
                    recordElements[i] = application.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, field.type);
                else
                    recordElements[i] = field.type.MakeDefaultValue();
            }

            return new RecordValue(application.sourcePosition, recordElements, this);
        }

        public override Value AnalyzeAndEvaluateDictionaryInstanceInEnvironment(ParseTreeImmutableDictionaryNode dictionary, Environment environment)
        {
            int fieldCount = fields.Length;
            Value[] recordFields = new Value[fieldCount];
            bool[] initializedRecordFields = new bool[fieldCount];

            // Initialize the dictionary fields.
            foreach(ParseTreeNode dictionaryElementNode in dictionary.elements)
            {
                if (!dictionaryElementNode.IsParseTreeAssociationNode())
                    throw new SysmelException(dictionaryElementNode.sourcePosition, "Expected an association with the record field name and value.");

                ParseTreeAssociationNode associationNode = (ParseTreeAssociationNode)dictionaryElementNode;
                string fieldName = associationNode.key.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();

                if(!fieldsByName.ContainsKey(fieldName))
                    throw new SysmelException(dictionaryElementNode.sourcePosition, "Record does not have a field named #" + fieldName + ".");

                FieldDefinition field = fieldsByName[fieldName];
                Value? fieldValue = null;
                if (associationNode.value == null)
                    fieldValue = field.type.MakeDefaultValue();
                else
                    fieldValue = associationNode.value.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, field.type);

                recordFields[field.index] = fieldValue;
                initializedRecordFields[field.index] = true;
            }
            // Initialize the other fields
            for(int i = 0; i < fieldCount; ++i)
            {
                if (!initializedRecordFields[i])
                    recordFields[i] = fields[i].type.MakeDefaultValue();
            }

            return new RecordValue(dictionary.sourcePosition, recordFields, this);
        }

        public override Value? LookupSelector(string selector)
        {
            string fieldName = selector;
            if (fieldName.EndsWith(':'))
                fieldName = fieldName.Substring(0, fieldName.Length - 1);
            if (fieldsByName.ContainsKey(fieldName))
                return fieldsByName[fieldName];

            return base.LookupSelector(selector);
        }

        public override FieldDefinition? LookupFieldName(string selector)
        {
            if (fieldsByName.ContainsKey(selector))
                return fieldsByName[selector];
            return null;
        }

        public override string ToString()
        {
            if (name == null)
                return "AnonRecord";
            return name;
        }
    }
}
