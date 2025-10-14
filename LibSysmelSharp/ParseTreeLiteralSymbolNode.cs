using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLiteralSymbolNode : ParseTreeNode
    {
        public string value = "";

        public override bool IsParseTreeLiteralSymbolNode()
        {
            return true;
        }
    }
}
