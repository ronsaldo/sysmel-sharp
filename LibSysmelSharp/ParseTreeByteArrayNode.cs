namespace LibSysmelSharp
{
    public class ParseTreeByteArrayNode : ParseTreeNode
    {
        public ParseTreeNode[] elements;
        public ParseTreeByteArrayNode(SourcePosition sourcePosition, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }

        public override bool IsParseTreeByteArrayNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            byte[] byteArrayData = new byte[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                Value element = elements[i].AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                if (!element.IsIntegerValue())
                    throw new SysmelException(elements[i].sourcePosition, "Expected an integral value.");
                byteArrayData[i] = (byte)element.AsInteger();
            }

            return new ByteArrayValue(sourcePosition, byteArrayData, Intrinsics.GetSingleton().ByteArrayType);
        }
    }
}
