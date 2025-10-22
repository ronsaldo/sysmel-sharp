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
            Value? nspaceValue = nspace.LookSymbol(symbol);
            if (nspaceValue != null)
                return nspaceValue;

            return parent.LookupRecursively(symbol);
        }

        public override Namespace? LookNamespace()
        {
            return nspace;
        }

        public override Value? LookEntityOwner()
        {
            return nspace;
        }
    }
}
