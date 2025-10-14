namespace LibSysmelSharp
{
    public class ParseTreeLiteralSymbolNode : ParseTreeNode
    {
        public string value = "";

        public ParseTreeLiteralSymbolNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralSymbolNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return new SymbolValue(sourcePosition, value, Intrinsics.GetSingleton().SymbolType);
        }
    }
}
