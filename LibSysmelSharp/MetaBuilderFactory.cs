using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public delegate Value MetaBuilderFactoryMakeInstance(SourcePosition? sourcePosition);

    public class MetaBuilderFactory : Value
    {
        public MetaBuilderFactoryMakeInstance makeInstance;

        public MetaBuilderFactory(SourcePosition? sourcePosition, MetaBuilderFactoryMakeInstance makeInstance) : base(sourcePosition)
        {
            this.makeInstance = makeInstance;
        }

        public override Value AnalyzeAndEvaluateIdentifierReferenceInEnvironment(ParseTreeIdentifierReferenceNode identifierNode, Environment environment)
        {
            return makeInstance(identifierNode.sourcePosition);
        }
    }
}
