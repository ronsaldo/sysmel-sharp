namespace LibSysmelSharp
{
    public class ParseTreeRecordDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode nameExpression;
        public ParseTreeNode fieldsExpression;

        public ParseTreeRecordDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode nameExpression, ParseTreeNode fieldsExpression) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.fieldsExpression = fieldsExpression;
        }

        public override bool IsParseTreeRecordDefinitionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string name = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();
            Value fieldValues = fieldsExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            if (!fieldValues.IsImmutableDictionaryValue())
                throw new SysmelException(sourcePosition, "Expected a dictionary with the record fields.");

            ImmutableDictionaryValue fieldValuesDict = (ImmutableDictionaryValue)fieldValues;
            int fieldCount = fieldValuesDict.elements.Length;
            FieldDefinition[] fields = new FieldDefinition[fieldCount];
            for(int i = 0; i < fieldCount; ++i)
            {
                AssociationValue assoc = fieldValuesDict.elements[i];
                if (!assoc.key.IsSymbolValue())
                    throw new SysmelException(assoc.key.sourcePosition, "Expected a symbol with the field name.");
                string fieldName = assoc.key.AsSymbol();

                SysmelType fieldType = Intrinsics.GetSingleton().DynamicType;
                if(!assoc.value.IsNilValue())
                {
                    if (!assoc.value.IsType())
                        throw new SysmelException(assoc.value.sourcePosition, "Expected a type for the field definition.");
                    fieldType = (SysmelType)assoc.value;
                }

                fields[i] = new FieldDefinition(assoc.sourcePosition, fieldName, fieldType);
            }

            RecordType recordType = new RecordType(sourcePosition, name, fields);
            environment.SetSymbolBinding(name, recordType);
            return recordType;
        }
    }
}
