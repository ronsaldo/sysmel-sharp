namespace LibSysmelSharp
{
    public class EnumMetaBuilder : NamedMetaBuilder
    {
        public ParseTreeNode? baseTypeExpresion = null;

        public EnumMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "baseType:" || selector == "baseType:values:" || selector == "values:";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (selector == "baseType:")
            {
                baseTypeExpresion = messageSend.arguments[0];
                return this;
            }
            else if (selector == "values:")
            {
                return new ParseTreeEnumDefinitionNode(messageSend.sourcePosition, nameExpression, baseTypeExpresion, messageSend.arguments[0])
                     .AnalyzeAndEvaluateInEnvironment(environment);
            }
            else if (selector == "baseType:values:")
            {
                return new ParseTreeEnumDefinitionNode(messageSend.sourcePosition, nameExpression, messageSend.arguments[0], messageSend.arguments[1])
                    .AnalyzeAndEvaluateInEnvironment(environment); ;
            }
            return this;
        }
    }
}
