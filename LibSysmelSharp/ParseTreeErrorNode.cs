namespace LibSysmelSharp
{
    public class ParseTreeErrorNode : ParseTreeNode
    {
        public string message = "";
        public ParseTreeNode? innerNode;

        public ParseTreeErrorNode(SourcePosition sourcePosition, string message, ParseTreeNode? innerNode) : base(sourcePosition)
        {
            this.message = message;
            this.innerNode = innerNode;
        }
        public override bool IsParseTreeErrorNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            throw new SysmelException(sourcePosition, "Parsing Error: " + message);
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            throw new SysmelException(sourcePosition, "Parsing Error: " + message);
        }
    }
}
