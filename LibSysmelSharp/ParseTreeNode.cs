namespace LibSysmelSharp
{
    public class ParseTreeNode : Value
    {
        public ParseTreeNode(SourcePosition sourcePosition) : base(sourcePosition) { }

        public override bool IsParseTreeNode()
        {
            return true;
        }
        public virtual bool IsParseTreeErrorNode()
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

        public virtual bool IsParseTreeAssignmentNode()
        {
            return false;
        }
        public virtual bool IsParseTreeAssociationNode()
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
    }
}
