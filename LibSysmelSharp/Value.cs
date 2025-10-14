namespace LibSysmelSharp
{
    public class Value
    {
        public SourcePosition? sourcePosition;

        public Value(SourcePosition? sourcePosition)
        {
            this.sourcePosition = sourcePosition;
        }

        public virtual Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return this;
        }

        public virtual bool AsBoolean()
        {
            throw new NotSupportedException();
        }

        public virtual long AsInteger()
        {
            throw new NotSupportedException();
        }
        public virtual double AsFloat()
        {
            throw new NotSupportedException();
        }

        public virtual string AsString()
        {
            throw new NotSupportedException();
        }
        public virtual string AsSymbol()
        {
            throw new NotSupportedException();
        }

        public virtual bool IsParseTreeNode()
        {
            return false;
        }

        public virtual bool IsType()
        {
            return false;
        }
        public virtual bool IsVoidValue()
        {
            return false;
        }

        public virtual bool IsNilValue()
        {
            return false;
        }

        public virtual bool IsBooleanValue()
        {
            return false;
        }

        public virtual bool IsIntegerValue()
        {
            return false;
        }

        public virtual bool IsFloatValue()
        {
            return false;
        }

        public virtual bool IsStringValue()
        {
            return false;
        }

        public virtual bool IsSymbolValue()
        {
            return false;
        }
        public virtual bool isMacroContextValue()
        {
            return false;
        }
        public virtual bool IsListValue()
        {
            return false;
        }
        public virtual bool IsTupleValue()
        {
            return false;
        }

        public virtual SysmelType GetSysmelType()
        {
            throw new NotImplementedException();
        }
    }
}
