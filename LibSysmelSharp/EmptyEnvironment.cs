namespace LibSysmelSharp
{
    public class EmptyEnvironment : Environment
    {
        public override Value? LookupRecursively(string symbol)
        {
            return null;
        }

        public override Namespace? LookNamespace()
        {
            return null;
        }
    }
}
