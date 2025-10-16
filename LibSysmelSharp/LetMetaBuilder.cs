using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class LetMetaBuilder : NamedMetaBuilder
    {
        public bool isMutable = false;
        public ParseTreeNode? typeExpression = null;

        public LetMetaBuilder(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public override bool IsSupportedMessageSelector(string selector)
        {
            return selector == "mutable" || selector == "type:";
        }

        public override Value ExpandMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, Environment environment)
        {
            if(selector == "mutable")
                isMutable = true;
            else if(selector == "type:")
                typeExpression = messageSend.arguments[0];
            return this;
        }

        public override Value AnalyzeAndEvaluateAssignmentInEnvironment(ParseTreeAssignmentNode assignmentNode, Environment environment)
        {
            ParseTreeVariableDefinitionNode varDefinition = new ParseTreeVariableDefinitionNode(assignmentNode.sourcePosition, nameExpression, typeExpression, assignmentNode.value, isMutable);
            return varDefinition.AnalyzeAndEvaluateInEnvironment(environment);
        }
    }
}
