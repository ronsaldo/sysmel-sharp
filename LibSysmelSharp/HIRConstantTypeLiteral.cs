namespace LibSysmelSharp
{
    internal class HIRConstantTypeLiteral: HIRConstant
    {
        public SysmelType typeLiteral;

        public HIRConstantTypeLiteral(SourcePosition? sourcePosition, SysmelType typeLiteral) : base(sourcePosition, typeLiteral.GetSysmelType())
        {
            this.typeLiteral = typeLiteral;
        }

        public override HIRValue AnalyzeAndBuildApplication(ParseTreeApplicationNode application, HIRBuilder builder)
        {
            return typeLiteral.AnalyzeAndBuildApplication(application, builder);
        }
    }
}
