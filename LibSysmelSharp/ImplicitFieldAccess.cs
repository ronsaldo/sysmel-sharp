namespace LibSysmelSharp
{
    public class ImplicitFieldAccess : Value
    {
        HIRValue aggregate;
        FieldDefinition fieldDefinition;

        public ImplicitFieldAccess(SourcePosition? sourcePosition, HIRValue aggregate, FieldDefinition fieldDefinition) : base(sourcePosition)
        {
            this.aggregate = aggregate;
            this.fieldDefinition = fieldDefinition;
        }

        public override HIRValue AnalyzeAndBuildIdentifierReference(ParseTreeIdentifierReferenceNode node, HIRBuilder builder)
        {
            return builder.ExtractFieldReference(node.sourcePosition, aggregate, fieldDefinition);
        }
    }
}
