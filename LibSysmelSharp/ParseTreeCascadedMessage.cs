using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeCascadedMessage : ParseTreeNode
    {
        public ParseTreeNode selector;
        public ParseTreeNode[] arguments;

        public ParseTreeCascadedMessage(SourcePosition? sourcePosition, ParseTreeNode selector, ParseTreeNode[] arguments) : base(sourcePosition)
        {
            this.selector = selector;
            this.arguments = arguments;
        }

        public override bool IsParseTreeCascadedMessageNode()
        {
            return true;
        }

        public override ParseTreeMessageSendNode AsParseTreeMessageSendNodeWithReceiver(ParseTreeNode receiver)
        {
            return new ParseTreeMessageSendNode(sourcePosition, receiver, selector, arguments);
        }

    }
}
