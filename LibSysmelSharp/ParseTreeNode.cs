namespace LibSysmelSharp
{
    public class ParseTreeNode : Value
    {
        public ParseTreeNode(SourcePosition? sourcePosition) : base(sourcePosition) { }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            throw new NotImplementedException();
        }

        public override Value AnalyzeAndEvaluateMacroArgumentInEnvironment(Environment environment, SysmelType expectedType)
        {
            if (expectedType == Intrinsics.GetSingleton().ParseTreeNodeType)
                return this;
            return base.AnalyzeAndEvaluateMacroArgumentInEnvironment(environment, expectedType);
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().ParseTreeNodeType;
        }

        public override bool IsParseTreeNode()
        {
            return true;
        }
        public virtual bool IsParseTreeErrorNode()
        {
            return false;
        }

        public virtual bool IsParseTreeBlockClosureNode()
        {
            return false;
        }

        public virtual bool IsParseTreeByteArrayNode()
        {
            return false;
        }

        public virtual bool IsParseTreeFunctionNode()
        {
            return false;
        }

        public virtual bool IsParseTreeFunctionalSignatureNode()
        {
            return false;
        }
        
        public virtual bool IsParseTreeIfSelectionNode()
        {
            return false;
        }
        public virtual bool IsParseTreeLexicalBlockNode()
        {
            return false;
        }

        public virtual bool IsParseTreeLiteralCharacterNode()
        {
            return false;
        }

        public virtual bool IsParseTreeLiteralFloatNode()
        {
            return false;
        }

        public virtual bool IsParseTreeLiteralIntegerNode()
        {
            return false;
        }

        public virtual bool IsParseTreeLiteralStringNode()
        {
            return false;
        }

        public virtual bool IsParseTreeLiteralSymbolNode()
        {
            return false;
        }
        public virtual bool IsParseTreeLiteralValueNode()
        {
            return false;
        }
        public virtual bool IsParseTreeLoadFileOnce()
        {
            return false;
        }

        public virtual bool IsParseTreeNamespaceNode()
        {
            return false;
        }
        public virtual bool IsParseTreePublicNode()
        {
            return false;
        }

        public virtual bool IsParseTreeSequenceNode()
        {
            return false;
        }
        public virtual bool IsParseTreeIdentifierReferenceNode()
        {
            return false;
        }

        public virtual bool IsParseTreeListNode()
        {
            return false;
        }


        public virtual bool IsParseTreeTupleNode()
        {
            return false;
        }
        public virtual bool IsParseTreeArgumentDefinitionNode()
        {
            return false;
        }

        public virtual bool IsParseTreeAssignmentNode()
        {
            return false;
        }
        public virtual bool IsParseTreeAssociationNode()
        {
            return false;
        }
        public virtual bool IsParseTreeImmutableDictionaryNode()
        {
            return false;
        }

        public virtual bool IsParseTreeApplicationNode()
        {
            return false;
        }
        public virtual bool IsParseTreeBinaryExpressionSequence()
        {
            return false;
        }

        public virtual bool IsParseTreeMessageSendNode()
        {
            return false;
        }

        public virtual bool IsParseTreeMessageCascadeNode()
        {
            return false;
        }

        public virtual bool IsParseTreeCascadedMessageNode()
        {
            return false;
        }

        public virtual bool IsParseTreeAddMethodNode()
        {
            return false;
        }

        public virtual bool IsParseTreeMethodNode()
        {
            return false;
        }

        public virtual bool IsParseTreeQuoteNode()
        {
            return false;
        }

        public virtual bool IsParseTreeQuasiQuoteNode()
        {
            return false;
        }

        public virtual bool IsParseTreeQuasiUnquoteNode()
        {
            return false;
        }
        public virtual bool IsParseTreeSpliceNode()
        {
            return false;
        }

        public virtual bool IsParseTreeVariableDefinitionNode()
        {
            return false;
        }

        public virtual bool IsParseTreeDoWhileLoopNode()
        {
            return false;
        }

        public virtual bool IsParseTreeWhileLoopNode()
        {
            return false;
        }

        public virtual bool IsParseTreeSwitchNode()
        {
            return false;
        }
        public virtual bool IsParseTreeClassDefinitionNode()
        {
            return false;
        }
        public virtual bool IsParseTreeEnumDefinitionNode()
        {
            return false;
        }
        public virtual bool IsParseTreeRecordDefinitionNode()
        {
            return false;
        }

        public virtual ParseTreeNode CopyWithPostfixReceiver(ParseTreeNode newReceiver)
        {
            throw new NotSupportedException();
        }

        public virtual void SplitReceiverForCascade(out ParseTreeNode outReceiver, out ParseTreeNode? firstCascadedMessage)
        {
            outReceiver = this;
            firstCascadedMessage = null;
        }

        public virtual ParseTreeMessageSendNode AsParseTreeMessageSendNodeWithReceiver(ParseTreeNode receiver)
        {
            throw new NotSupportedException();
        }
    }
}
