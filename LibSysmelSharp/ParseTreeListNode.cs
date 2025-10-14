using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeListNode : ParseTreeNode
    {
        public ParseTreeNode[] elements;
        public ParseTreeListNode(SourcePosition sourcePosition, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }

        public override bool IsParseTreeListNode()
        {
            return true;
        }
    }
}
