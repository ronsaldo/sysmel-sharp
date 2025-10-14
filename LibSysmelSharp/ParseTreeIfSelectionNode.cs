using System.Linq.Expressions;

namespace LibSysmelSharp
{
    public class ParseTreeIfSelectionNode : ParseTreeNode
    {
        public ParseTreeNode condition;
        public ParseTreeNode? trueExpression;
        public ParseTreeNode? falseExpression;

        public ParseTreeIfSelectionNode(SourcePosition? sourcePosition, ParseTreeNode condition, ParseTreeNode? trueExpression, ParseTreeNode? falseExpression) : base(sourcePosition)
        {
            this.condition = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }
        public override bool IsParseTreeIfSelectionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value conditionValue = condition.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().BooleanType);
            if(conditionValue.AsBoolean())
            {
                if (trueExpression != null)
                    return trueExpression.AnalyzeAndEvaluateInEnvironment(environment);
                else
                    return VoidValue.GetSingleton();
            }
            else
            {
                if (falseExpression != null)
                    return falseExpression.AnalyzeAndEvaluateInEnvironment(environment);
                else
                    return VoidValue.GetSingleton();
            }
        }
    }
}
