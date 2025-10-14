using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeFunctionalSignatureNode : ParseTreeNode
    {
        public ParseTreeArgumentDefinitionNode[] argumentDefinitions;
        public ParseTreeNode? resultTypeExpression;
        public string? selector;

        public ParseTreeFunctionalSignatureNode(SourcePosition sourcePosition, ParseTreeArgumentDefinitionNode[] argumentDefinitions, ParseTreeNode? resultTypeExpression, string? selector) : base(sourcePosition)
        {
            this.argumentDefinitions = argumentDefinitions;
            this.resultTypeExpression = resultTypeExpression;
            this.selector = selector;
        }

        public override bool IsParseTreeFunctionalSignatureNode()
        {
            return true;
        }
    }
}
