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
    }
}
