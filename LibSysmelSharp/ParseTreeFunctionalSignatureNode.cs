using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeFunctionalSignatureNode : ParseTreeNode
    {
        public ParseTreeArgumentDefinitionNode[] argumentDefinitions;
        public ParseTreeNode? resultTypeExpression;
        public string? selector;

        public ParseTreeFunctionalSignatureNode(SourcePosition? sourcePosition, ParseTreeArgumentDefinitionNode[] argumentDefinitions, ParseTreeNode? resultTypeExpression, string? selector) : base(sourcePosition)
        {
            this.argumentDefinitions = argumentDefinitions;
            this.resultTypeExpression = resultTypeExpression;
            this.selector = selector;
        }

        public override bool IsParseTreeFunctionalSignatureNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            FunctionalSignatureAnalysisEnvironment analysisEnvironment = new FunctionalSignatureAnalysisEnvironment(environment);
            
            int argumentCount = argumentDefinitions.Length;
            ArgumentDefinitionValue[] argumentValues = new ArgumentDefinitionValue[argumentCount];
            for (int i = 0; i < argumentCount; ++i)
                argumentValues[i] = (ArgumentDefinitionValue)argumentDefinitions[i].AnalyzeAndEvaluateInEnvironment(analysisEnvironment);

            Value resultType = Intrinsics.GetSingleton().DynamicType;
            if(resultTypeExpression != null)
            {
                resultType = resultTypeExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(analysisEnvironment);
                if (!resultType.IsType())
                    throw new SysmelException(resultTypeExpression.sourcePosition, "Expected a type expression.");
            }

            return new DependentFunctionType(sourcePosition, selector, argumentValues, resultType);
        }
    }
}
