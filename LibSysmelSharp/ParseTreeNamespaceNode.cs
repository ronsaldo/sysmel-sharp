namespace LibSysmelSharp
{
    public class ParseTreeNamespaceNode : ParseTreeNode
    {
        public ParseTreeNode nameExpression;
        public ParseTreeNode definitionBody;

        public ParseTreeNamespaceNode(SourcePosition? sourcePosition, ParseTreeNode nameExpression, ParseTreeNode definitionBody) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.definitionBody = definitionBody;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            throw new NotImplementedException();
        }
    }
}
