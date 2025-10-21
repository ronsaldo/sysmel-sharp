using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            FunctionValue functionValue = new FunctionValue(sourcePosition, null, false, false, functionTypeWithSignature, environment, body);
            functionValue.EnsureAnalysis();
            return functionValue;
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            DependentFunctionType functionTypeWithSignature = (DependentFunctionType)functionalSignature.AnalyzeAndEvaluateInEnvironment(builder.environment);
            FunctionValue functionValue = new FunctionValue(sourcePosition, null, false, false, functionTypeWithSignature, builder.environment, body);
            functionValue.EnsureAnalysis();

            if (functionValue.hirFunction.captures.Count == 0)
                return functionValue.hirFunction;

            int captureCount = functionValue.hirFunction.captures.Count;
            HIRValue[] captureSources = new HIRValue[captureCount];
            for (int i = 0; i < captureCount; ++i)
            {
                HIRValue? captureSource = functionValue.hirFunction.captures[i].sourceValue;
                Debug.Assert(captureSource != null);
                captureSources[i] = captureSource;
            }

            return builder.MakeClosure(sourcePosition, functionValue.hirFunction, captureSources);
        }
    }
}
