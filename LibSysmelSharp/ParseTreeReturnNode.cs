namespace LibSysmelSharp
{
    public class ParseTreeReturnNode : ParseTreeNode
    {
        public ParseTreeNode resultExpression;

        public ParseTreeReturnNode(SourcePosition? sourcePosition, ParseTreeNode resultExpression) : base(sourcePosition)
        {
            this.resultExpression = resultExpression;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value resultValue = resultExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            throw new SysmelReturnException(resultValue);
        }

    }
}
