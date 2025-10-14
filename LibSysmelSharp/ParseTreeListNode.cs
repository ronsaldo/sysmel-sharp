using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeListNode : ParseTreeNode
    {
        public ParseTreeNode[] elements;
        public ParseTreeListNode(SourcePosition sourcePosition, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }

        public override bool IsParseTreeListNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value[] values = new Value[elements.Length];
            for (int i = 0; i < elements.Length; i++)
                values[i] = elements[i].AnalyzeAndEvaluateInEnvironment(environment);

            return new ListValue(sourcePosition, values, Intrinsics.GetSingleton().DynamicListType);
        }
    }
}
