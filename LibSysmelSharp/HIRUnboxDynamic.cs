namespace LibSysmelSharp
{
    public class HIRUnboxDynamic : HIRInstruction
    {
        public HIRValue boxedValue;

        public HIRUnboxDynamic(SourcePosition? sourcePosition, HIRValue boxedValue, SysmelType unboxedType) : base(sourcePosition, unboxedType, null)
        {
            this.boxedValue = boxedValue;
        }

        public override bool IsUnboxDynamic()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := dynamicUnbox " + boxedValue.ToString() + " withType " + type.ToString());
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value valueToUnbox = boxedValue.GetValueInEvaluationContext(context);
            if (!type.IsSatisfiedBy(valueToUnbox.GetSysmelType()))
                throw new SysmelException(sourcePosition, "Cannot unbox from Dynamic into " + type.ToString());

            context.SetCurrentInstructionValue(valueToUnbox);
        }
    }
}
