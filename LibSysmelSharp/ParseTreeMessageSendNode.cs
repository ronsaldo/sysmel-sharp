using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeMessageSendNode : ParseTreeNode
    {
        public ParseTreeNode receiver;
        public ParseTreeNode selector;
        public ParseTreeNode[] arguments;

        public ParseTreeMessageSendNode(SourcePosition? sourcePosition, ParseTreeNode receiver, ParseTreeNode selector, ParseTreeNode[] arguments) : base(sourcePosition)
        {
            this.receiver = receiver;
            this.selector = selector;
            this.arguments = arguments;
        }

        public override bool IsParseTreeMessageSendNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value receiverValue = receiver.AnalyzeAndEvaluateInEnvironment(environment);
            Value selectorValue = selector.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType);
            return receiverValue.AnalyzeAndEvaluateMessageSendInEnvironment(this, selectorValue.AsSymbol(), environment);
        }

        public override void SplitReceiverForCascade(out ParseTreeNode outReceiver, out ParseTreeNode? firstCascadedMessage)
        {
            outReceiver = receiver;
            firstCascadedMessage = new ParseTreeCascadedMessage(sourcePosition, selector, arguments);
        }
    }
}
