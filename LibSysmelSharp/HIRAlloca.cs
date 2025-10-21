using System.Reflection.Metadata;

namespace LibSysmelSharp
{
    public class HIRAlloca : HIRInstruction
    {
        public SysmelType valueType;

        public HIRAlloca(SourcePosition? sourcePosition, SysmelType valueType, SysmelType type) :base(sourcePosition, type, null)
        {
            this.valueType = valueType;
        }

        public override bool IsAllocaInstruction()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := alloca " + valueType.ToString());
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            MutableValueBox boxValue = new MutableValueBox(sourcePosition, valueType.MakeDefaultValue(), valueType.MutableBox());
            ReferenceLikeValue referenceValue = new ReferenceLikeValue(sourcePosition, boxValue, 0, valueType.Ref());
            context.SetCurrentInstructionValue(referenceValue);
        }
    }
}
