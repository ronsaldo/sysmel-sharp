namespace LibSysmelSharp
{
    public class ParseTreeWhileLoopNode : ParseTreeNode
    {
        ParseTreeNode condition;
        ParseTreeNode? bodyExpression;
        ParseTreeNode? continueExpression;

        public ParseTreeWhileLoopNode(SourcePosition? sourcePosition, ParseTreeNode condition, ParseTreeNode? bodyExpression, ParseTreeNode? continueExpression) : base(sourcePosition)
        {
            this.condition = condition;
            this.bodyExpression = bodyExpression;
            this.continueExpression = continueExpression;
        }

        public override bool IsParseTreeWhileLoopNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            SysmelType booleanType = Intrinsics.GetSingleton().BooleanType;
            while(condition.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, booleanType).AsBoolean())
            {
                if (bodyExpression != null)
                    bodyExpression.AnalyzeAndEvaluateInEnvironment(environment);

                if (continueExpression != null)
                    continueExpression.AnalyzeAndEvaluateInEnvironment(environment);
            }

            return VoidValue.GetSingleton();
        }
    }
}
