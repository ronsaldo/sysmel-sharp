using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibSysmelSharp
{
    public class ParseTreeAssociationNode : ParseTreeNode
    {
        public ParseTreeNode key;
        public ParseTreeNode? value;
        public ParseTreeAssociationNode(SourcePosition sourcePosition, ParseTreeNode key, ParseTreeNode? value) : base(sourcePosition)
        {
            this.sourcePosition = sourcePosition;
            this.key = key;
            this.value = value;
        }
                public override bool IsParseTreeAssociationNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value keyValue = key.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            Value valueValue = NilValue.GetSingleton();
            if(value != null)
                valueValue = value.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);

            if(keyValue.IsType() && valueValue.IsType())
                return new AssociationType(sourcePosition, (SysmelType)keyValue, (SysmelType)valueValue);

            AssociationType associationType = new AssociationType(sourcePosition, keyValue.GetSysmelType(), valueValue.GetSysmelType());
            return new AssociationValue(sourcePosition, associationType, keyValue, valueValue);
        }
    }
}
