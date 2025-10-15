
namespace LibSysmelSharp
{
    public class PointerLikeValue : Value
    {
        public Value storage;
        public int fieldIndex;
        public SysmelType type;

        public PointerLikeValue(SourcePosition? sourcePosition, Value storage, int fieldIndex, SysmelType type) : base(sourcePosition)
        {
            this.storage = storage;
            this.fieldIndex = fieldIndex;
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override bool IsPointerLikeValue()
        {
            return true;
        }
        public override Value LoadValue()
        {
            return storage.LoadValueWithFieldIndex(fieldIndex);
        }
        
        public override void StoreValue(Value valueToStore)
        {
            storage.StoreValueWithFieldIndex(valueToStore, fieldIndex);
        }

    }
}
