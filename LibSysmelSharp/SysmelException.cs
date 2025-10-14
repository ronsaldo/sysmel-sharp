namespace LibSysmelSharp
{
    public class SysmelException : Exception
    {
        public SysmelException(SourcePosition? sourcePosition, string message) :
            base(sourcePosition != null
                    ? (sourcePosition.ToString() + ": " + message)
                    : message)
        {
        }
    }
}
