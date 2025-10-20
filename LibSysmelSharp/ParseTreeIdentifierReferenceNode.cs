using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LibSysmelSharp
{
    public class ParseTreeIdentifierReferenceNode : ParseTreeNode
    {
        public string value = "";
        public ParseTreeIdentifierReferenceNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeIdentifierReferenceNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value? foundValue = environment.LookupRecursively(value);
            if(foundValue == null)
                throw new SysmelException(sourcePosition, "Failed to find identifier #" + value);
            return foundValue.AnalyzeAndEvaluateIdentifierReferenceInEnvironment(this, environment);
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            Debug.Assert(builder.environment != null);
            Value? foundValue = builder.environment.LookupRecursively(value);
            if (foundValue == null)
                throw new SysmelException(sourcePosition, "Failed to find identifier #" + value);

            return foundValue.AnalyzeAndBuildIdentifierReference(this, builder);
        }

        public override ParseTreeArgumentDefinitionNode ParseAsArgumentDefinitionInEnvironment(Environment environment)
        {
            return new ParseTreeArgumentDefinitionNode(sourcePosition, value, null);
        }

        public override ParseTreeArgumentDefinitionNode[] ParseAsArgumentDefinitionsInEnvironment(Environment environment)
        {
            return new ParseTreeArgumentDefinitionNode[] { this.ParseAsArgumentDefinitionInEnvironment(environment) };
        }
    }
}
