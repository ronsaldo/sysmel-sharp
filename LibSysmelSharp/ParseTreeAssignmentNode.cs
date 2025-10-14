using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeAssignmentNode : ParseTreeNode
    {
        public ParseTreeNode store;
        public ParseTreeNode value;

        public ParseTreeAssignmentNode(SourcePosition sourcePosition, ParseTreeNode store, ParseTreeNode value) : base(sourcePosition)
        {
            this.store = store;
            this.value = value;
        }

        public override bool IsParseTreeAssignmentNode()
        {
            return true;
        }
    }
}
