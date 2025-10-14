using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeArgumentDefinitionNode : ParseTreeNode
    {
        public string? name;
        public ParseTreeNode? typeExpression;

        public ParseTreeArgumentDefinitionNode(SourcePosition sourcePosition, string? name, ParseTreeNode? typeExpression) : base(sourcePosition)
        {
            this.name = name;
            this.typeExpression = typeExpression;
        }
    }
}
