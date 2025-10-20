using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeBlockClosureNode : ParseTreeNode
    {
        public ParseTreeFunctionalSignatureNode functionalSignature;
        public ParseTreeNode body;

        public ParseTreeBlockClosureNode(SourcePosition sourcePosition, ParseTreeFunctionalSignatureNode functionalSignature, ParseTreeNode body) : base(sourcePosition)
        {
            this.functionalSignature = functionalSignature;
            this.body = body;
        }

        public override bool IsParseTreeBlockClosureNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            DependentFunctionType functionTypeWithSignature = (DependentFunctionType)functionalSignature.AnalyzeAndEvaluateInEnvironment(environment);
            FunctionValue functionValue = new FunctionValue(sourcePosition, null, functionTypeWithSignature, environment, body);
            functionValue.EnsureAnalysis();
            return functionValue;
        }
    }
}
