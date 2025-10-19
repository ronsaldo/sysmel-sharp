namespace LibSysmelSharp
{
    public class ParseTreeNamespaceNode : ParseTreeNode
    {
        public ParseTreeNode nameExpression;
        public ParseTreeNode? definitionBody;

        public ParseTreeNamespaceNode(SourcePosition? sourcePosition, ParseTreeNode nameExpression, ParseTreeNode? definitionBody) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.definitionBody = definitionBody;
        }

        public override bool IsParseTreeNamespaceNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string name = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();
            Namespace? ownerNamespace = environment.LookNamespace();
            if (ownerNamespace == null)
                throw new SysmelException(sourcePosition, "No owner namespace available in this context.");

            Namespace childNamespace = ownerNamespace.GetOrCreateChildNamespace(sourcePosition, name);
            NamespaceEnvironment childEnvironment = new NamespaceEnvironment(childNamespace, environment);
            LexicalEnvironment childLexicalEnvironment = new LexicalEnvironment(childEnvironment);

            if(definitionBody != null)
                definitionBody.AnalyzeAndEvaluateInEnvironment(childLexicalEnvironment);
            return childNamespace;
        }
    }
}
