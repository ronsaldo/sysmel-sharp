namespace LibSysmelSharp
{
    public class HIRLoadInstruction : HIRInstruction
    {
        public HIRValue storage;

        public HIRLoadInstruction(SourcePosition? sourcePosition, SysmelType type, HIRValue storage) : base(sourcePosition, type, null)
        {
            this.storage = storage;
        }

        public override bool IsLoadInstruction()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := load " + storage.ToString());
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value pointerLikeValue = storage.GetValueInEvaluationContext(context);
            Value loadedValue = pointerLikeValue.LoadValue();
            context.SetCurrentInstructionValue(loadedValue);
        }
    }
}
