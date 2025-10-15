using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeImmutableDictionaryNode : ParseTreeNode
    {
        public ParseTreeNode? dictionaryType;
        public ParseTreeNode[] elements;

        public ParseTreeImmutableDictionaryNode(SourcePosition? sourcePosition, ParseTreeNode? dictionaryType, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.dictionaryType = dictionaryType;
            this.elements = elements;
        }

        public override bool IsParseTreeImmutableDictionaryNode()
        {
            return true;
        }
    }
}
