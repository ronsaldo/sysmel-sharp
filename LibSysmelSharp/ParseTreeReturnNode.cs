using System;

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

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            // TODO: Cast into the required type.
            HIRValue resultValue = resultExpression.AnalyzeAndBuildWithDecayedType(builder);
            if (resultValue.GetSysmelType().IsVoidType())
                builder.ReturnVoid(sourcePosition);
            else
                builder.ReturnValue(sourcePosition, resultValue);

            return builder.GetVoidLiteral(sourcePosition);
        }
    }
}
