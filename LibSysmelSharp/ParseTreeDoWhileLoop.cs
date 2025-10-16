namespace LibSysmelSharp
{
    public class ParseTreeDoWhileLoopNode : ParseTreeNode
    {
        ParseTreeNode? bodyExpression;
        ParseTreeNode? continueExpression;
        ParseTreeNode condition;

        public ParseTreeDoWhileLoopNode(SourcePosition? sourcePosition, ParseTreeNode? bodyExpression, ParseTreeNode? continueExpression, ParseTreeNode condition) : base(sourcePosition)
        {
            this.bodyExpression = bodyExpression;
            this.continueExpression = continueExpression;
            this.condition = condition;
        }

        public override bool IsDoWhileLoopNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            SysmelType booleanType = Intrinsics.GetSingleton().BooleanType;
            do
            {
                if (bodyExpression != null)
                    bodyExpression.AnalyzeAndEvaluateInEnvironment(environment);

                if (continueExpression!= null)
                    continueExpression.AnalyzeAndEvaluateInEnvironment(environment);

            } while (condition.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, booleanType).AsBoolean());


            return VoidValue.GetSingleton();
        }
    }
}
