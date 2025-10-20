using System.Diagnostics;

namespace LibSysmelSharp
{
    public class ParseTreeSwitchNode : ParseTreeNode
    {
        public ParseTreeNode value;
        public ParseTreeNode cases;

        public ParseTreeSwitchNode(SourcePosition? sourcePosition, ParseTreeNode value, ParseTreeNode cases) : base(sourcePosition)
        {
            this.value = value;
            this.cases = cases;
        }

        public override bool IsParseTreeSwitchNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value evaluatedValue = value.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            if (!cases.IsParseTreeImmutableDictionaryNode())
                throw new SysmelException(cases.sourcePosition, "Expected a dictionary with the switch cases.");

            ParseTreeImmutableDictionaryNode switchCasesDict = (ParseTreeImmutableDictionaryNode)cases;
            foreach(ParseTreeNode caseNode in switchCasesDict.elements)
            {
                if(!caseNode.IsParseTreeAssociationNode())
                    throw new SysmelException(caseNode.sourcePosition, "Expected an association with the switch case."); ;

                ParseTreeAssociationNode caseAssociation = (ParseTreeAssociationNode)caseNode;
                Value caseKey = caseAssociation.key.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                bool isDefaultCase = caseKey.IsSymbolValue() && caseKey.AsSymbol() == "_";
                if (isDefaultCase || caseKey.Equals(evaluatedValue))
                {
                    if (caseAssociation.value != null)
                        return caseAssociation.value.AnalyzeAndEvaluateInEnvironment(environment);
                    else
                        return VoidValue.GetSingleton();
                }
            }

            throw new SysmelException(sourcePosition, "Switch does not have a default case.");
        }

        public SysmelType MergeResultTypePair(SysmelType left, SysmelType right)
        {
            if (left.Equals(right))
                return left;
            
            return Intrinsics.GetSingleton().VoidType;
        }

        public SysmelType MergeTypesOfResultValues(List<HIRValue> casesResultValues, HIRValue switchDefaultResult)
        {
            SysmelType resultType = switchDefaultResult.GetSysmelType();
            foreach (HIRValue caseResult in casesResultValues)
                resultType = MergeResultTypePair(resultType, caseResult.GetSysmelType());
            return resultType;
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRValue valueIR = value.AnalyzeAndBuildWithDecayedType(builder);
            if (!cases.IsParseTreeImmutableDictionaryNode())
                throw new SysmelException(cases.sourcePosition, "Expected a dictionary with the switch cases.");

            List<HIRValue> caseKeys = new List<HIRValue>();
            List<HIRBasicBlock> caseDestinations = new List<HIRBasicBlock>();
            List<HIRBuilder> caseBuilders = new List<HIRBuilder>();
            List<HIRValue> casesResultValues = new List<HIRValue>();

            HIRBasicBlock? switchDefaultBlock = null;
            HIRBuilder? switchDefaultBuilder = null;
            HIRValue? switchDefaultResult = builder.GetVoidLiteral(sourcePosition);

            HIRBasicBlock switchMergeBlock = new HIRBasicBlock(sourcePosition, "switchMerge");

            ParseTreeImmutableDictionaryNode switchCasesDict = (ParseTreeImmutableDictionaryNode)cases;
            foreach (ParseTreeNode caseNode in switchCasesDict.elements)
            {
                if (!caseNode.IsParseTreeAssociationNode())
                    throw new SysmelException(caseNode.sourcePosition, "Expected an association with the switch case."); ;

                ParseTreeAssociationNode caseAssociation = (ParseTreeAssociationNode)caseNode;
                HIRValue caseKey = caseAssociation.key.AnalyzeAndBuildWithDecayedType(builder);
                if(caseKey.IsDefaultCaseKey())
                {
                    switchDefaultBlock = new HIRBasicBlock(caseAssociation.sourcePosition, "switchDefault");
                    builder.function.AddBasicBlock(switchDefaultBlock);

                    switchDefaultBuilder = builder.CopyWithBasicBlock(switchDefaultBlock);
                    if (caseAssociation.value != null)
                        switchDefaultResult = caseAssociation.value.AnalyzeAndBuildWithDecayedType(switchDefaultBuilder);
                }
                else
                {
                    HIRBasicBlock caseBasicBlock = new HIRBasicBlock(caseAssociation.sourcePosition, "switchCase");
                    builder.function.AddBasicBlock(caseBasicBlock);
                    
                    HIRBuilder caseBuilder = builder.CopyWithBasicBlock(caseBasicBlock);
                    HIRValue caseValue = caseBuilder.GetVoidLiteral(caseAssociation.sourcePosition);
                    if (caseAssociation.value != null)
                        caseValue = caseAssociation.value.AnalyzeAndBuildWithDecayedType(caseBuilder);

                    caseKeys.Add(caseKey);
                    caseDestinations.Add(caseBasicBlock);
                    caseBuilders.Add(caseBuilder);
                    casesResultValues.Add(caseValue);
                }
            }

            if(switchDefaultBlock == null)
                throw new SysmelException(sourcePosition, "Switch does not have a default case.");
            Debug.Assert(switchDefaultBuilder != null);

            builder.Switch(sourcePosition, valueIR, caseKeys.ToArray(), caseDestinations.ToArray(), switchDefaultBlock);

            // Switch merge
            builder.function.AddBasicBlock(switchMergeBlock);
            builder.basicBlock = switchMergeBlock;

            // Merge the result types.
            SysmelType mergedResultType = MergeTypesOfResultValues(casesResultValues, switchDefaultResult);
            HIRValue mergedResult = builder.GetVoidLiteral(sourcePosition);
            if(!mergedResultType.IsVoidType())
            {
                HIRPhi mergedPhi = builder.Phi(sourcePosition, mergedResultType, "switchPhi");
                mergedResult = mergedPhi;

                for (int i = 0; i < caseBuilders.Count(); ++i)
                    caseBuilders[i].PhiSource(sourcePosition, mergedPhi, casesResultValues[i]);
                switchDefaultBuilder.PhiSource(sourcePosition, mergedPhi, switchDefaultResult);
            }

            // Finish the cases basic blocks.
            for (int i = 0; i < caseBuilders.Count(); ++i)
            {
                if(!caseBuilders[i].IsLastTerminator())
                    caseBuilders[i].Branch(sourcePosition, switchMergeBlock);
            }

            if (!switchDefaultBuilder.IsLastTerminator())
                switchDefaultBuilder.Branch(sourcePosition, switchMergeBlock);

            return mergedResult;
        }
    }
}
