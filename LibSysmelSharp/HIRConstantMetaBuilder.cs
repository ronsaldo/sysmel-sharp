using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRConstantMetaBuilder : HIRConstant
    {
        public MetaBuilder metaBuilder;

        public HIRConstantMetaBuilder(SourcePosition? sourcePosition, MetaBuilder metaBuilder) : base(sourcePosition, Intrinsics.GetSingleton().MetaBuilderType)
        {
            this.metaBuilder = metaBuilder;
        }

        public override HIRValue AnalyzeAndBuildAssignment(ParseTreeAssignmentNode assignmentNode, HIRBuilder builder)
        {
            return metaBuilder.AnalyzeAndBuildAssignment(assignmentNode, builder);
        }

        public override HIRValue AnalyzeAndBuildMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, HIRBuilder builder)
        {
            return metaBuilder.AnalyzeAndBuildMessageSendInEnvironment(messageSend, selector, builder);
        }
    }
}
