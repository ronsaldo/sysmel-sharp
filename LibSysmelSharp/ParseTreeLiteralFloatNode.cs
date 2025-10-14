using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLiteralFloatNode : ParseTreeNode
    {
        public double value;

        public override bool IsParseTreeLiteralFloatNode()
        {
            return true;
        }
    }
}
