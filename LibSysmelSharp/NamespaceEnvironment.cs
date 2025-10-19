namespace LibSysmelSharp
{
    public class NamespaceEnvironment  : Environment
    {
        public Namespace nspace;
        public Environment parent;

        public NamespaceEnvironment(Namespace nspace, Environment parent)
        {
            this.nspace = nspace;
            this.parent = parent;
        }

        public override Value? LookupRecursively(string symbol)
        {
            Value? intrinsicValue = nspace.LookSymbol(symbol);
            if (intrinsicValue != null)
                return intrinsicValue;

            return parent.LookupRecursively(symbol);
        }

        public override Namespace? LookNamespace()
        {
            return nspace;
        }
    }
}
