using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeApplicationNode : ParseTreeNode
    {
        public ParseTreeNode functional;
        public ParseTreeNode[] arguments;

        public ParseTreeApplicationNode(SourcePosition sourcePosition, ParseTreeNode functional, ParseTreeNode[] arguments) : base(sourcePosition)
        {
            this.sourcePosition = sourcePosition;
            this.functional = functional;
            this.arguments = arguments;
        }

        public override bool IsParseTreeApplicationNode()
        {
            return true;
        }
    }
}
