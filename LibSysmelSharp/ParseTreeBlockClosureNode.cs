using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeBlockClosureNode : ParseTreeNode
    {
        public ParseTreeFunctionalSignatureNode functionalSignature;
        public ParseTreeNode body;

        public ParseTreeBlockClosureNode(SourcePosition sourcePosition, ParseTreeFunctionalSignatureNode functionalSignature, ParseTreeNode body) : base(sourcePosition)
        {
            this.functionalSignature = functionalSignature;
            this.body = body;
        }

        public override bool IsParseTreeBlockClosureNode()
        {
            return true;
        }
    }
}
