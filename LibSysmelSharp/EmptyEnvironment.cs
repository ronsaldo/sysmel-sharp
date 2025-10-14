namespace LibSysmelSharp
{
    public class EmptyEnvironment : Environment
    {
        public override Value? LookupRecursively(string symbol)
        {
            return null;
        }
    }
}
