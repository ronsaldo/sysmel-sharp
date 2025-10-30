namespace LibSysmelSharp
{
    public class AbstractFunctionType : SysmelType
    {
        public AbstractFunctionType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsFunctionType()
        {
            return true;
        }
        public override SysmelType? GetSuperType()
        {
            return Intrinsics.GetSingleton().FunctionType;
        }

        public override Value? LookupSelector(string selector)
        {
            SysmelType? superType = GetSuperType();
            if (superType != null)
                return superType.LookupSelector(selector);
            return null;
        }

        public override Value MakeDefaultValue()
        {
            return NilValue.GetSingleton();
        }

    }
}
