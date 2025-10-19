namespace LibSysmelSharp
{
    public class ParseTreePublicNode : ParseTreeNode
    {
        public ParseTreeNode exportList;

        public ParseTreePublicNode(SourcePosition? sourcePosition, ParseTreeNode exportList) : base(sourcePosition)
        {
            this.exportList = exportList;
        }

        public override bool IsParseTreePublicNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value? currentEntityOwner = environment.LookEntityOwner();
            if(currentEntityOwner == null)
                throw new SysmelException(sourcePosition, "This location does not accept public entities.");

            Value exportListValue = exportList.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            if (!exportListValue.IsListValue())
                throw new SysmelException(sourcePosition, "Expected a list of public entities.");

            ListValue castedExport = (ListValue)exportListValue;
            foreach(Value exportedValue in castedExport.elements)
            {
                string? exportedName = exportedValue.GetName();
                if (exportedName == null)
                    continue;

                exportedValue.SetOwner(currentEntityOwner);
                currentEntityOwner.SetSymbolBinding(exportedName, exportedValue);
            }

            return VoidValue.GetSingleton();
        }
    }
}
