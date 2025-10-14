namespace LibSysmelSharp
{
    public class EmptyEnvironment : Environment
    {
        public override Value? lookupRecursively(string symbol)
        {
            return null;
        }
    }
}
