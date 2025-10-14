namespace LibSysmelSharp
{
    public class ParseTreeNode : Value
    {
        public override bool IsParseTreeNode()
        {
            return true;
        }
        public virtual bool IsParseTreeErrorNode()
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

        public virtual bool IsParseTreeTupleNode()
        {
            return false;
        }

    }
}
