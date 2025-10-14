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
    }
}
