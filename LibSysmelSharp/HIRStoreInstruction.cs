namespace LibSysmelSharp
{
    public class HIRStoreInstruction : HIRInstruction
    {
        public HIRValue storage;
        public HIRValue valueToStore;

        public HIRStoreInstruction(SourcePosition? sourcePosition, HIRValue storage, HIRValue valueToStore) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.storage = storage;
            this.valueToStore = valueToStore;
        }

        public override bool IsLoadInstruction()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  store " + valueToStore.ToString() + " in " + storage.ToString());
        }
    }
}
