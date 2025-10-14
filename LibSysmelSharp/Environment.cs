namespace LibSysmelSharp
{
    public abstract class Environment
    {
        public abstract Value? lookupRecursively(string symbol);
    }
}
