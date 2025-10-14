using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLexicalBlockNode : ParseTreeNode
    {
        public ParseTreeNode body;

        public ParseTreeLexicalBlockNode(SourcePosition sourcePosition, ParseTreeNode body) : base(sourcePosition)
        {
            this.body = body;
        }
        public override bool IsParseTreeLexicalBlockNode()
        {
            return true;
        }
    }
}
