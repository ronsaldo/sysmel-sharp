using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeErrorNode : ParseTreeNode
    {
        public string message = "";

        public override bool IsParseTreeErrorNode()
        {
            return true;
        }
    }
}
