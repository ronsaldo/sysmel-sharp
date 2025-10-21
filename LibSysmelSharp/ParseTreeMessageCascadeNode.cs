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
        public ParseTreeNode[] cascadedMessages;

        public ParseTreeMessageCascadeNode(SourcePosition? sourcePosition, ParseTreeNode receiver, ParseTreeNode[] cascadedMessages) : base(sourcePosition)
        {
            this.receiver = receiver;
            this.cascadedMessages = cascadedMessages;
        }

        public override bool IsParseTreeMessageCascadeNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value receiverValue = receiver.AnalyzeAndEvaluateInEnvironment(environment);
            ParseTreeLiteralValueNode receiverValueNode = new ParseTreeLiteralValueNode(receiver.sourcePosition, receiverValue);

            Value result = receiverValue;
            foreach(ParseTreeNode cascadedMessage in cascadedMessages)
            {
                ParseTreeNode cascadedMessageWithReceiver = cascadedMessage.AsParseTreeMessageSendNodeWithReceiver(receiverValueNode);
                result = cascadedMessageWithReceiver.AnalyzeAndEvaluateInEnvironment(environment);
            }

            return result;
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRValue receiverValue = receiver.AnalyzeAndBuild(builder);
            ParseTreeLiteralValueNode receiverValueNode = new ParseTreeLiteralValueNode(receiver.sourcePosition, receiverValue);
            
            HIRValue result = receiverValue;
            foreach (ParseTreeNode cascadedMessage in cascadedMessages)
            {
                ParseTreeNode cascadedMessageWithReceiver = cascadedMessage.AsParseTreeMessageSendNodeWithReceiver(receiverValueNode);
                result = cascadedMessageWithReceiver.AnalyzeAndBuild(builder);
            }

            return result;
        }
    }
}
