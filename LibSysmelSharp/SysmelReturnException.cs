namespace LibSysmelSharp
{
    public class SysmelReturnException : Exception
    {
        public Value resultValue;
        public SysmelReturnException(Value resultValue)
        {
            this.resultValue = resultValue;
        }
    }
}
