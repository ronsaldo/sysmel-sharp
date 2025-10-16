using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeFunctionNode : ParseTreeNode
    {
        public ParseTreeNode? nameExpression;
        public ParseTreeFunctionalSignatureNode functionalSignature;
        public ParseTreeNode body;

        public ParseTreeFunctionNode(SourcePosition? sourcePosition, ParseTreeNode? nameExpression, ParseTreeFunctionalSignatureNode functionalSignature, ParseTreeNode body) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.functionalSignature = functionalSignature;
            this.body = body;
        }

        public override bool IsParseTreeFunctionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            DependentFunctionType functionTypeWithSignature = (DependentFunctionType)functionalSignature.AnalyzeAndEvaluateInEnvironment(environment);
            string? nameSymbolValue = null;
            if (nameExpression != null)
            {
                Value nameValue = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType);
                nameSymbolValue = nameValue.AsSymbol();
            }

            FunctionValue functionValue = new FunctionValue(sourcePosition, nameSymbolValue, functionTypeWithSignature, environment, body);
            if (nameSymbolValue != null)
                environment.SetSymbolBinding(nameSymbolValue, functionValue);
            return functionValue;
        }
    }
}
