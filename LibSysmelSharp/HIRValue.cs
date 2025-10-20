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

        public override HIRValue AnalyzeAndBuildIdentifierReference(ParseTreeIdentifierReferenceNode node, HIRBuilder builder)
        {
            return this;
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
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

        public virtual bool IsAllocaInstruction()
        {
            return false;
        }

        public virtual bool IsBranchInstruction()
        {
            return false;
        }
        public virtual bool IsConditionalBranchInstruction()
        {
            return false;
        }

        public virtual bool IsCallFunctionInstruction()
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

        public virtual bool IsReturnValue()
        {
            return false;
        }

        public virtual bool IsReturnVoid()
        {
            return false;
        }

        public virtual bool IsTerminator()
        {
            return false;
        }

        public virtual bool IsImportedFunction()
        {
            return true;
        }
    }
}
