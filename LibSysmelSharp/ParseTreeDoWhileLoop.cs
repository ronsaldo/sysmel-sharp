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

        public override bool IsParseTreeDoWhileLoopNode()
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

                if (continueExpression != null)
                    continueExpression.AnalyzeAndEvaluateInEnvironment(environment);

            } while (condition.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, booleanType).AsBoolean());


            return VoidValue.GetSingleton();
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRBasicBlock loopHeader = new HIRBasicBlock(sourcePosition, "doLoopHeader");
            HIRBasicBlock loopContinueWith = new HIRBasicBlock(sourcePosition, "doLoopContinueWith");
            HIRBasicBlock loopCondition = new HIRBasicBlock(sourcePosition, "doLoopCondition");
            HIRBasicBlock loopMerge = new HIRBasicBlock(sourcePosition, "doLoopMerge");

            builder.Branch(sourcePosition, loopHeader);

            // Loop header.
            builder.basicBlock = loopHeader;
            builder.function.AddBasicBlock(loopHeader);

            if (bodyExpression != null)
                bodyExpression.AnalyzeAndBuild(builder);

            if (!builder.IsLastTerminator())
                builder.Branch(sourcePosition, loopContinueWith);

            // Loop continue
            builder.basicBlock = loopContinueWith;
            builder.function.AddBasicBlock(loopContinueWith);

            if(continueExpression != null)
                continueExpression.AnalyzeAndBuild(builder);

            if (!builder.IsLastTerminator())
                builder.Branch(sourcePosition, loopCondition);

            // Loop condition.
            builder.basicBlock = loopCondition;
            builder.function.AddBasicBlock(loopCondition);

            HIRValue conditionValue = condition.AnalyzeAndBuildWithExpectedType(builder, Intrinsics.GetSingleton().BooleanType);
            builder.ConditionalBranch(sourcePosition, conditionValue, loopHeader, loopMerge);

            // Loop merge.
            builder.basicBlock = loopMerge;
            builder.function.AddBasicBlock(loopMerge);

            return builder.GetVoidLiteral(sourcePosition);
        }
    }
}