namespace LibSysmelSharp
{
    public class ClassMetaBuilder: NamedMetaBuilder
    {
        public bool isMutable = false;
        public ParseTreeNode? typeExpression = null;

        public ClassMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "withFields:" || selector == "withSuperclass:fields:";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (selector == "withFields:")
            {
                return new ParseTreeClassDefinitionNode(messageSend.sourcePosition, nameExpression, null, null, messageSend.arguments[0])
                        .AnalyzeAndEvaluateInEnvironment(environment);
            }
            else if (selector == "withSuperclass:fields:")
            {
                return new ParseTreeClassDefinitionNode(messageSend.sourcePosition, nameExpression, messageSend.arguments[0], null, messageSend.arguments[1])
                        .AnalyzeAndEvaluateInEnvironment(environment);
            }
            return this;
        }
    }
}
