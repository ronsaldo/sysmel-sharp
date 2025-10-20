using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibSysmelSharp
{
    public class ParseTreeAssignmentNode : ParseTreeNode
    {
        public ParseTreeNode store;
        public ParseTreeNode value;

        public ParseTreeAssignmentNode(SourcePosition sourcePosition, ParseTreeNode store, ParseTreeNode value) : base(sourcePosition)
        {
            this.store = store;
            this.value = value;
        }

        public override bool IsParseTreeAssignmentNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value storeValue = store.AnalyzeAndEvaluateInEnvironment(environment);
            return storeValue.AnalyzeAndEvaluateAssignmentInEnvironment(this, environment);
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRValue storageValue = store.AnalyzeAndBuild(builder);
            return storageValue.AnalyzeAndBuildAssignment(this, builder);
        }
    }
}
