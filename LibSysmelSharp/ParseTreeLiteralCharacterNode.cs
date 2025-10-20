namespace LibSysmelSharp
{
    public class ParseTreeLiteralCharacterNode : ParseTreeNode
    {
        public uint value;

        public ParseTreeLiteralCharacterNode(SourcePosition sourcePosition, uint value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralCharacterNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return new IntegerValue(sourcePosition, value, Intrinsics.GetSingleton().CharacterType);
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return builder.AddLiteral(sourcePosition, AnalyzeAndEvaluateInEnvironment(builder.environment));
        }
    }
}
