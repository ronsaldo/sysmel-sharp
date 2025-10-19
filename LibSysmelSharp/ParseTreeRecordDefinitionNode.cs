namespace LibSysmelSharp
{
    public class ParseTreeRecordDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode? nameExpression;
        public ParseTreeNode fieldsExpression;

        public ParseTreeRecordDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode? nameExpression, ParseTreeNode fieldsExpression) : base(sourcePosition)
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
            string? name = null;
            if (nameExpression != null)
                name = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();

            Value fieldValues = fieldsExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            if (!fieldValues.IsImmutableDictionaryValue())
                throw new SysmelException(sourcePosition, "Expected a dictionary with the record fields.");

            ImmutableDictionaryValue fieldValuesDict = (ImmutableDictionaryValue)fieldValues;
            FieldDefinition[] fields = fieldValuesDict.EvaluateAsFieldListInEnviroment(environment);

            RecordType recordType = new RecordType(sourcePosition, name, fields);
            if(name != null)
                environment.SetSymbolBinding(name, recordType);
            return recordType;
        }
    }
}
