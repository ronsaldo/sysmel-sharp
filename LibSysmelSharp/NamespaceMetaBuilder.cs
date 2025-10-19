namespace LibSysmelSharp
{
    public class NamespaceMetaBuilder : NamedMetaBuilder
    {

        public NamespaceMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "definition:";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (selector == "definition:")
                return new ParseTreeNamespaceNode(messageSend.sourcePosition, nameExpression, messageSend.arguments[0]).AnalyzeAndEvaluateInEnvironment(environment);

            return this;
        }

        public override Value AnalyzeAndEvaluateAssignmentInEnvironment(ParseTreeAssignmentNode assignmentNode, Environment environment)
        {
            return new ParseTreeNamespaceNode(assignmentNode.sourcePosition, nameExpression, assignmentNode.value).AnalyzeAndEvaluateInEnvironment(environment);
        }
    }
}
