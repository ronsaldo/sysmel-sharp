using System.Linq.Expressions;

namespace LibSysmelSharp
{
    public class ParseTreeBinaryExpressionSequence : ParseTreeNode
    {
        public ParseTreeNode[] operands;

        public ParseTreeBinaryExpressionSequence(SourcePosition sourcePosition, ParseTreeNode[] operands) : base(sourcePosition)
        {
            this.operands = operands;
        }

        public override bool IsParseTreeBinaryExpressionSequence()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return ExpandAsMessageSendSequence().AnalyzeAndEvaluateInEnvironment(environment);
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return ExpandAsMessageSendSequence().AnalyzeAndBuild(builder);
        }

        public ParseTreeNode ExpandAsMessageSendSequence()
        {
            ParseTreeNode result = operands[0];
            for(int i = 1; i < operands.Length; i += 2)
            {
                ParseTreeNode op = operands[i];
                ParseTreeNode operand = operands[i + 1];

                result = new ParseTreeMessageSendNode(op.sourcePosition, result, op, new ParseTreeNode[] { operand });
            }

            return result;
        }
    }
}
