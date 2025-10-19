namespace LibSysmelSharp
{
    public abstract class Environment
    {
        public virtual void SetSymbolBinding(string symbol, Value binding)
        {
            throw new NotSupportedException();
        }

        public abstract Value? LookupRecursively(string symbol);

        public abstract Namespace? LookNamespace();
    }
}
