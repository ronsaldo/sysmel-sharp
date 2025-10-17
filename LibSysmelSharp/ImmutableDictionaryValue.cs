using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ImmutableDictionaryValue : Value
    {
        public ImmutableDictionaryType type;
        public AssociationValue[] elements;

        public ImmutableDictionaryValue(SourcePosition? sourcePosition, ImmutableDictionaryType type, AssociationValue[] elements) : base(sourcePosition)
        {
            this.type = type;
            this.elements = elements;
        }

        public override bool IsImmutableDictionaryValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public FieldDefinition[] EvaluateAsFieldListInEnviroment(Environment environment)
        {
            int fieldCount = elements.Length;
            FieldDefinition[] fields = new FieldDefinition[fieldCount];
            for (int i = 0; i < fieldCount; ++i)
            {
                AssociationValue assoc = elements[i];
                if (!assoc.key.IsSymbolValue())
                    throw new SysmelException(assoc.key.sourcePosition, "Expected a symbol with the field name.");
                string fieldName = assoc.key.AsSymbol();

                SysmelType fieldType = Intrinsics.GetSingleton().DynamicType;
                if (!assoc.value.IsNilValue())
                {
                    if (!assoc.value.IsType())
                        throw new SysmelException(assoc.value.sourcePosition, "Expected a type for the field definition.");
                    fieldType = (SysmelType)assoc.value;
                }

                fields[i] = new FieldDefinition(assoc.sourcePosition, fieldName, fieldType);
            }

            return fields;
        }
    }
}
