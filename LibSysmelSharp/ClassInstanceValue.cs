using System.Text;

namespace LibSysmelSharp
{
    public class ClassInstanceValue : Value
    {
        public Value[] elements;
        public BehaviorType type;

        public ClassInstanceValue(SourcePosition? sourcePosition, Value[] elements, BehaviorType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsClassInstanceValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override Value LoadValueWithFieldIndex(int fieldIndex)
        {
            return elements[fieldIndex];
        }

        public override void StoreValueWithFieldIndex(Value valueToStore, int fieldIndex)
        {
            elements[fieldIndex] = valueToStore;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(type.ToString());
            builder.Append("(...)");
            return builder.ToString();
        }
    }
}
