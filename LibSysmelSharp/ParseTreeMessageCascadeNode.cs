using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeMessageCascadeNode : ParseTreeNode
    {
        public ParseTreeNode receiver;
        public ParseTreeNode[] cascadedMessage;

        public ParseTreeMessageCascadeNode(SourcePosition? sourcePosition, ParseTreeNode receiver, ParseTreeNode[] cascadedMessage) : base(sourcePosition)
        {
            this.receiver = receiver;
            this.cascadedMessage = cascadedMessage;
        }

        public override bool IsParseTreeMessageCascadeNode()
        {
            return true;
        }
    }
}
