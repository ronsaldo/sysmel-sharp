
namespace LibSysmelSharp
{
    public class SimpleType : SysmelType
    {
        public Dictionary<string, Value> methodDict;
        public List<Value> methodList;
        public SysmelType? superType;

        public SimpleType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
            methodDict = new Dictionary<string, Value>();
            methodList = new List<Value>();
        }

        public override SysmelType? GetSuperType()
        {
            return superType;
        }

        public override void WithSelectorAddMethod(string selector, Value method)
        {
            methodDict[selector] = method;
            methodList.Add(method);

            method.SetOwner(this);
        }

        public override Value? LookupSelector(string selector)
        {
            if (methodDict.ContainsKey(selector))
                return methodDict[selector];

            if (superType != null)
                return superType.LookupSelector(selector);
            return null;
        }
    }
}
