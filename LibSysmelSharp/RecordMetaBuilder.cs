namespace LibSysmelSharp
{
    public class RecordMetaBuilder : NamedMetaBuilder
    {
        public bool isMutable = false;
        public ParseTreeNode? typeExpression = null;

        public RecordMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "withFields:";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (selector == "withFields:")
            {
                return new ParseTreeRecordDefinitionNode(messageSend.sourcePosition, nameExpression, messageSend.arguments[0])
                        .AnalyzeAndEvaluateInEnvironment(environment);
            }
            return this;
        }
    }
}
