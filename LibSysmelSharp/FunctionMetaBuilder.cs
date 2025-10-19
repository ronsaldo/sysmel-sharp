using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class FunctionMetaBuilder : NamedMetaBuilder
    {
        public ParseTreeArgumentDefinitionNode[] argumentDefinitions = new ParseTreeArgumentDefinitionNode[0];
        public ParseTreeNode? resultTypeExpression = null;

        public FunctionMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "=>";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if (selector == "=>")
                resultTypeExpression = messageSend.arguments[0];
            return this;
        }

        public override Value AnalyzeAndEvaluateApplicationInEnvironment(ParseTreeApplicationNode application, Environment environment)
        {
            int argumentCount = application.arguments.Length;
            argumentDefinitions = new ParseTreeArgumentDefinitionNode[argumentCount];
            for(int i = 0; i < argumentCount; ++i)
                argumentDefinitions[i] = application.arguments[i].ParseAsArgumentDefinitionInEnvironment(environment);
            return this;
        }

        public override Value AnalyzeAndEvaluateAssignmentInEnvironment(ParseTreeAssignmentNode assignmentNode, Environment environment)
        {
            ParseTreeFunctionalSignatureNode signature = new ParseTreeFunctionalSignatureNode(assignmentNode.sourcePosition, argumentDefinitions, resultTypeExpression, name, false);
            ParseTreeFunctionNode functionNode = new ParseTreeFunctionNode(sourcePosition, nameExpression, signature, assignmentNode.value);
            return functionNode.AnalyzeAndEvaluateInEnvironment(environment);
        }
    }
}
