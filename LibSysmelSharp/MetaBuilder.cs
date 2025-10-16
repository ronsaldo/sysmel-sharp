using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibSysmelSharp
{
    public class MetaBuilder : Value
    {
        public MetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsMetaBuilder()
        {
            return true;
        }

        public virtual bool IsSupportedMessageSelector(string selector)
        {
            return false;
        }

        public virtual Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            return this;
        }

        public override Value AnalyzeAndEvaluateApplicationInEnvironment(ParseTreeApplicationNode application, Environment environment)
        {
            throw new SysmelException(sourcePosition, "Metabuilder does not support functional application");
        }

        public override Value AnalyzeAndEvaluateMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (IsSupportedMessageSelector(selector))
                return ExpandMessageSendInEnvironment(messageSend, selector, environment);
            return base.AnalyzeAndEvaluateMessageSendInEnvironment(messageSend, selector, environment);
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().MetaBuilderType;
        }
    }
}
