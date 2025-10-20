namespace LibSysmelSharp
{
    public class HIRValue : Value
    {
        public SysmelType type;

        public HIRValue(SourcePosition? sourcePosition, SysmelType type) : base(sourcePosition)
        {
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override HIRValue AnalyzeAndBuilderIdentifierReference(ParseTreeIdentifierReferenceNode node, HIRBuilder builder)
        {
            return this;
        }

        public override bool IsHIRValue()
        {
            return true;
        }


        public virtual bool IsArgumentValue()
        {
            return false;
        }

        public virtual bool IsCaptureValue()
        {
            return false;
        }

        public virtual bool IsConstant()
        {
            return false;
        }

        public virtual bool IsConstantLiteralValue()
        {
            return false;
        }
        public virtual bool IsLoadInstruction()
        {
            return false;
        }
        public virtual bool IsStoreInstruction()
        {
            return false;
        }

        public virtual bool IsBoxDynamic()
        {
            return false;
        }

        public virtual bool IsUnboxDynamic()
        {
            return false;
        }

    }
}
