using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class FieldDefinition : Value
    {
        public string name;
        public int index = 0;
        public SysmelType type;

        public FieldDefinition(SourcePosition? sourcePosition, string name, SysmelType type) : base(sourcePosition)
        {
            this.name = name;
            this.type = type;
        }
        
        public override bool IsFieldDefinition()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().FieldType;
        }

        public override Value AnalyzeAndEvaluateMessageSendInEnvironmentWithMethod(ParseTreeMessageSendNode messageSend, string selector, Environment environment, Value receiver, Value method)
        {
            int argumentCount = messageSend.arguments.Length;
            if (argumentCount == 0)
            {
                return receiver.LoadValueWithFieldIndex(index);
            }
            else if(argumentCount == 1)
            {
                Value valueToStore = messageSend.arguments[0].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, type);
                receiver.StoreValueWithFieldIndex(valueToStore, index);
                return receiver;
            }

            throw new SysmelException(messageSend.sourcePosition, "Field access has a wrong number of arguments.");
        }
    }
}
