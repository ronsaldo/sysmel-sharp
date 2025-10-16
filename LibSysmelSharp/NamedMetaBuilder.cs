using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class NamedMetaBuilder : MetaBuilder
    {
        public string? name;
        public ParseTreeLiteralSymbolNode? nameExpression;

        public NamedMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }
        public override Value AnalyzeAndEvaluateMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if(name == null && messageSend.arguments.Length == 0)
            {
                name = selector;
                nameExpression = new ParseTreeLiteralSymbolNode(messageSend.selector.sourcePosition, selector);
                return this;
            }
            return base.AnalyzeAndEvaluateMessageSendInEnvironment(messageSend, selector, environment);
        }
    }
}
