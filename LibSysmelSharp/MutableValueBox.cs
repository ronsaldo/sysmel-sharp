namespace LibSysmelSharp
{
    internal class MutableValueBox : Value
    {
        public Value currentValue;
        public MutableBoxType type;

        public MutableValueBox(SourcePosition? sourcePosition, Value currentValue, MutableBoxType type) : base(sourcePosition)
        {
            this.currentValue = currentValue;
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override Value LoadValueWithFieldIndex(int fieldIndex)
        {
            return currentValue;
        }
        public override void StoreValueWithFieldIndex(Value valueToStore, int fieldIndex)
        {
            currentValue = valueToStore;
        }
    }
}
