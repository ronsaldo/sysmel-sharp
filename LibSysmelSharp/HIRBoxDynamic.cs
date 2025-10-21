namespace LibSysmelSharp
{
    public class HIRBoxDynamic : HIRInstruction
    {
        public HIRValue valueToBox;

        public HIRBoxDynamic(SourcePosition? sourcePosition, HIRValue valueToBox) : base(sourcePosition, Intrinsics.GetSingleton().DynamicType, null)
        {
            this.valueToBox = valueToBox;
        }

        public override bool IsBoxDynamic()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := dynamicBox " + valueToBox.ToString());
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            context.SetCurrentInstructionValue(valueToBox.GetValueInEvaluationContext(context));
        }
    }
}
