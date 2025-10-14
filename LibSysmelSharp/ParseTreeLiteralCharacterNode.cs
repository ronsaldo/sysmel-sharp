using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLiteralCharacterNode : ParseTreeNode
    {
        public uint value;

        public override bool IsParseTreeLiteralCharacterNode()
        {
            return true;
        }
    }
}
