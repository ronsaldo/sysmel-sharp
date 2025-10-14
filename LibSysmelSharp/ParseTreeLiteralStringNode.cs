using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLiteralStringNode : ParseTreeNode
    {
        public string value = "";

        public override bool IsParseTreeLiteralStringNode()
        {
            return true;
        }
    }
}
