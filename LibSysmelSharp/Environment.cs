namespace LibSysmelSharp
{
    public abstract class Environment
    {
        public abstract Value? LookupRecursively(string symbol);
    }
}
