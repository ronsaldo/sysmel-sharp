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
    }
}
