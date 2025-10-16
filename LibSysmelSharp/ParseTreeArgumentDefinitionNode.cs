using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeArgumentDefinitionNode : ParseTreeNode
    {
        public string? name;
        public ParseTreeNode? typeExpression;

        public ParseTreeArgumentDefinitionNode(SourcePosition sourcePosition, string? name, ParseTreeNode? typeExpression) : base(sourcePosition)
        {
            this.name = name;
            this.typeExpression = typeExpression;
        }

        public override bool IsParseTreeArgumentDefinitionNode()
        {
            return true;
        }

        public override ParseTreeArgumentDefinitionNode ParseAsArgumentDefinitionInEnvironment(Environment environment)
        {
            return this;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            FunctionalSignatureAnalysisEnvironment analysisEnvironment = (FunctionalSignatureAnalysisEnvironment)environment;

            // Evaluate the argument type.
            SysmelType? typeValue = null;
            if (typeExpression != null)
                typeValue = (SysmelType)typeExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
            if (typeValue == null)
                typeValue = Intrinsics.GetSingleton().DynamicType;
            
            ArgumentDefinitionValue argumentValue = new ArgumentDefinitionValue(sourcePosition, name, typeValue, analysisEnvironment.GetArgumentCount());
            analysisEnvironment.AddArgument(argumentValue);
            return argumentValue;
        }
    }
}
