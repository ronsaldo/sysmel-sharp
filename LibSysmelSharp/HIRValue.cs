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
        public virtual bool IsSuperArgumentValue()
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
        public virtual bool IsSwitchInstruction()
        {
            return false;
        }

        public virtual bool IsSendMessageInstruction()
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

        public virtual bool IsConstantTypeLiteral()
        {
            return false;
        }

        public virtual Value AsLiteralValue()
        {
            throw new NotSupportedException();
        }

        public virtual bool IsExtractFieldReference()
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

        public virtual bool IsAssertInstruction()
        {
            return false;
        }

        public virtual bool IsRuntimeErrorInstruction()
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

        public virtual bool IsInstruction()
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
            return false;
        }

        public virtual bool IsPhi()
        {
            return false;
        }

        public virtual bool IsPhiSource()
        {
            return false;
        }

        public virtual bool IsDefaultCaseKey()
        {
            return false;
        }

        public virtual Value GetValueInEvaluationContext(HIRFunctionActivationContext context)
        {
            throw new NotImplementedException();
        }
        public virtual void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
