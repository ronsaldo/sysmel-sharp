using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeApplicationNode : ParseTreeNode
    {
        public ParseTreeNode functional;
        public ParseTreeNode[] arguments;

        public ParseTreeApplicationNode(SourcePosition sourcePosition, ParseTreeNode functional, ParseTreeNode[] arguments) : base(sourcePosition)
        {
            this.sourcePosition = sourcePosition;
            this.functional = functional;
            this.arguments = arguments;
        }

        public override bool IsParseTreeApplicationNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value functionalValue = functional.AnalyzeAndEvaluateInEnvironment(environment);
            return functionalValue.AnalyzeAndEvaluateApplicationInEnvironment(this, environment);
        }

        public override ParseTreeArgumentDefinitionNode ParseAsArgumentDefinitionInEnvironment(Environment environment)
        {
            Debug.Assert(arguments.Length == 1);
            Debug.Assert(functional.IsParseTreeIdentifierReferenceNode());

            ParseTreeIdentifierReferenceNode functionalIdentifier = (ParseTreeIdentifierReferenceNode)functional;
            string argumentName = functionalIdentifier.value;
            if (argumentName.EndsWith(":"))
                argumentName = argumentName.Substring(0, argumentName.Length - 1);

            return new ParseTreeArgumentDefinitionNode(sourcePosition, argumentName, arguments[0]);
        }

        public override ParseTreeArgumentDefinitionNode[] ParseAsArgumentDefinitionsInEnvironment(Environment environment)
        {
            return new ParseTreeArgumentDefinitionNode[] { this.ParseAsArgumentDefinitionInEnvironment(environment) };
        }
    }
}
