namespace LibSysmelSharp
{
    public class ParseTreeAssertNode : ParseTreeNode
    {
        public ParseTreeNode assertionCondition;

        public ParseTreeAssertNode(SourcePosition? sourcePosition, ParseTreeNode assertionCondition) : base(sourcePosition)
        {
            this.assertionCondition = assertionCondition;
        }

        public override bool IsParseTreeAssertNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            bool assertionValue = assertionCondition.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().BooleanType).AsBoolean();
            if (!assertionValue)
            {
                SourcePosition? assertionPosition = assertionCondition.sourcePosition;
                if(assertionPosition != null)
                    throw new SysmelException(sourcePosition, "Assertion Failure: " + assertionPosition.GetText());
                else
                    throw new SysmelException(sourcePosition, "Assertion Failure");
            }

            return VoidValue.GetSingleton();
        }
    }
}
