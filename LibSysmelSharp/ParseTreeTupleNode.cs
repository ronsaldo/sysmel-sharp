using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeTupleNode : ParseTreeNode
    {
        public ParseTreeNode[] elements = new ParseTreeNode[0];

        public override bool IsParseTreeTupleNode()
        {
            return true;
        }
    }
}
