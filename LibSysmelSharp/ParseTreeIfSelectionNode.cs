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

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRBasicBlock trueDestination = new HIRBasicBlock(sourcePosition, "ifTrue");
            HIRBasicBlock falseDestination = new HIRBasicBlock(sourcePosition, "ifFalse");
            HIRBasicBlock mergeDestination = new HIRBasicBlock(sourcePosition, "isMerge");

            HIRValue conditionValue = condition.AnalyzeAndBuildWithExpectedType(builder, Intrinsics.GetSingleton().BooleanType); ;
            builder.ConditionalBranch(sourcePosition, conditionValue, trueDestination, falseDestination);

            // True destination
            HIRBuilder trueBuilder = builder.CopyWithBasicBlock(trueDestination);
            trueBuilder.function.AddBasicBlock(trueDestination);

            HIRValue? trueResult = null;
            if(trueExpression != null)
                trueResult = trueExpression.AnalyzeAndBuildWithDecayedType(trueBuilder);

            // False destination.
            HIRBuilder falseBuilder = builder.CopyWithBasicBlock(falseDestination);
            falseBuilder.function.AddBasicBlock(falseDestination);

            HIRValue? falseResult = null;
            if (falseExpression != null)
                falseResult = falseExpression.AnalyzeAndBuildWithDecayedType(falseBuilder);

            // Merge destination
            builder.function.AddBasicBlock(mergeDestination);
            builder.basicBlock = mergeDestination;

            // Merge the results using phi nodes.
            HIRValue? mergedResult = builder.GetVoidLiteral(sourcePosition);
            if(trueResult != null && falseResult != null)
            {
                SysmelType trueType = trueResult.GetSysmelType();
                SysmelType falseType = falseResult.GetSysmelType();

                if(!trueType.IsVoidType() && trueType.Equals(falseType))
                {
                    HIRPhi phiNode = builder.Phi(sourcePosition, trueType, "ifMergePhi");
                    trueBuilder.PhiSource(sourcePosition, phiNode, trueResult);
                    falseBuilder.PhiSource(sourcePosition, phiNode, falseResult);
                    mergedResult = phiNode;
                }
            }

            trueBuilder.Branch(sourcePosition, mergeDestination);
            falseBuilder.Branch(sourcePosition, falseDestination);

            return mergedResult;
        }
    }
}
