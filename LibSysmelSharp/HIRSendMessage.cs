using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibSysmelSharp
{
    public class HIRSendMessage : HIRInstruction
    {
        public string selector;
        public HIRValue receiver;
        public HIRValue[] messageArguments;

        public HIRSendMessage(SourcePosition? sourcePosition, HIRValue receiver, string selector, HIRValue[] messageArguments, SysmelType resultType) : base(sourcePosition, resultType, null)
        {
            this.receiver = receiver;
            this.selector = selector;
            this.messageArguments = messageArguments;
        }

        public override bool IsSendMessageInstruction()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.Write("  " + ToString() + " := send " + receiver.ToString() + " message " + selector);
            foreach (HIRValue value in messageArguments)
                Console.Write(", " + value.ToString());
            Console.WriteLine();
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value receiverValue = receiver.GetValueInEvaluationContext(context);

            int argumentCount = messageArguments.Length;
            Value[] callArgumentValues = new Value[argumentCount];
            for (int i = 0; i < argumentCount; ++i)
                callArgumentValues[i] = messageArguments[i].GetValueInEvaluationContext(context);
            
            Value messageResult = receiverValue.PerformWithArguments(selector, callArgumentValues);
            context.SetCurrentInstructionValue(messageResult);
        }
    }
}
