using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeImmutableDictionaryNode : ParseTreeNode
    {
        public ParseTreeNode? dictionaryType;
        public ParseTreeNode[] elements;

        public ParseTreeImmutableDictionaryNode(SourcePosition? sourcePosition, ParseTreeNode? dictionaryType, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.dictionaryType = dictionaryType;
            this.elements = elements;
        }

        public override bool IsParseTreeImmutableDictionaryNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            // TODO: Implement this case.
            Debug.Assert(dictionaryType == null);

            int elementCount = elements.Length;
            if(elementCount == 0)
                return new ImmutableDictionaryValue(sourcePosition, Intrinsics.GetSingleton().DynamicImmutableDictionaryType, new AssociationValue[0]);

            AssociationValue[] elementValues = new AssociationValue[elementCount];
            for(int i = 0; i < elementCount; ++i)
            {
                Value element = elements[i].AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                if (!element.IsAssociationValue())
                    throw new SysmelException(elements[i].sourcePosition, "Immutable dictionaries are constructed with associations.");

                elementValues[i] = (AssociationValue)element;
            }

            Debug.Assert(elementValues[0].GetSysmelType().IsAssociationType());
            AssociationType assocType = (AssociationType)elementValues[0].GetSysmelType();
            //TODO: make sure that all of the elements have matching association type.
            
            ImmutableDictionaryType dictType = new ImmutableDictionaryType(sourcePosition, assocType);
            return new ImmutableDictionaryValue(sourcePosition, dictType, elementValues);
        }
    }
}