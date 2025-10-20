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

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRBasicBlock loopHeader = new HIRBasicBlock(sourcePosition, "loopHeader");
            HIRBasicBlock loopBody = new HIRBasicBlock(sourcePosition, "loopBody");
            HIRBasicBlock loopContinueWith = new HIRBasicBlock(sourcePosition, "loopContinueWith");
            HIRBasicBlock loopMerge = new HIRBasicBlock(sourcePosition, "loopMerge");

            builder.Branch(sourcePosition, loopHeader);

            // Loop header
            builder.basicBlock = loopHeader;
            builder.function.AddBasicBlock(loopHeader);

            HIRValue loopCondition = condition.AnalyzeAndBuildWithExpectedType(builder, Intrinsics.GetSingleton().BooleanType);
            builder.ConditionalBranch(sourcePosition, loopCondition, loopBody, loopMerge);

            // Loop body.
            builder.basicBlock = loopBody;
            builder.function.AddBasicBlock(loopBody);

            if (bodyExpression != null)
                bodyExpression.AnalyzeAndBuild(builder);

            if (!builder.IsLastTerminator())
                builder.Branch(sourcePosition, loopContinueWith);

            // Loop continue
            builder.basicBlock = loopContinueWith;
            builder.function.AddBasicBlock(loopContinueWith);

            if (continueExpression != null)
                continueExpression.AnalyzeAndBuild(builder);

            if (!builder.IsLastTerminator())
                builder.Branch(sourcePosition, loopHeader);

            // Loop merge.
            builder.basicBlock = loopMerge;
            builder.function.AddBasicBlock(loopMerge);

            return builder.GetVoidLiteral(sourcePosition);
        }
    }
}
