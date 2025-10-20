using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeSequenceNode : ParseTreeNode
    {
        public ParseTreeNode[] expressions;

        public ParseTreeSequenceNode(SourcePosition sourcePosition, ParseTreeNode[] expressions) : base(sourcePosition)
        {
            this.expressions = expressions;
        }

        public override bool IsParseTreeSequenceNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value result = VoidValue.GetSingleton();
            foreach (ParseTreeNode expression in expressions)
                result = expression.AnalyzeAndEvaluateInEnvironment(environment);
            return result;
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRValue result = builder.GetVoidLiteral(sourcePosition);
            foreach (ParseTreeNode expression in expressions)
                result = expression.AnalyzeAndBuild(builder);
            return result;
        }

        public override ParseTreeArgumentDefinitionNode[] ParseAsArgumentDefinitionsInEnvironment(Environment environment)
        {
            int argumentCount = expressions.Length;
            ParseTreeArgumentDefinitionNode[] result = new ParseTreeArgumentDefinitionNode[argumentCount];
            for (int i = 0; i < argumentCount; ++i)
                result[i] = expressions[i].ParseAsArgumentDefinitionInEnvironment(environment);
            return result;
        }
    }
}
