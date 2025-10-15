namespace LibSysmelSharp
{
    public class ParseTreeTupleNode : ParseTreeNode
    {
        public ParseTreeNode[] elements;
        public ParseTreeTupleNode(SourcePosition sourcePosition, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }

        public override bool IsParseTreeTupleNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            if (elements.Length == 0)
                return VoidValue.GetSingleton();

            Value[] values = new Value[elements.Length];
            SysmelType[] valueTypes = new SysmelType[elements.Length];
            bool isAllTypes = true;
            for (int i = 0; i < elements.Length; i++)
            {
                values[i] = elements[i].AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                isAllTypes = isAllTypes && values[i].IsType();
                valueTypes[i] = values[i].GetSysmelType();
            }

            if (isAllTypes)
            {
                SysmelType[] valuesAsTypes = new SysmelType[elements.Length];
                for (int i = 0; i < elements.Length; i++)
                    valuesAsTypes[i] = (SysmelType)values[i];
                return new TupleType(sourcePosition, valuesAsTypes);
            }

            TupleType tupleType = new TupleType(sourcePosition, valueTypes);
            return new TupleValue(sourcePosition, values, tupleType);
        }
    }
}
