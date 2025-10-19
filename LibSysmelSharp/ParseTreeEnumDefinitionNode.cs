namespace LibSysmelSharp
{
    public class ParseTreeEnumDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode? nameExpression;
        public ParseTreeNode? baseType;
        public ParseTreeNode values;

        public ParseTreeEnumDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode? nameExpression, ParseTreeNode? baseType, ParseTreeNode values) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.baseType = baseType;
            this.values = values;
        }

        public override bool IsParseTreeEnumDefinitionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string? name = null;
            if(nameExpression != null)
                name = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();
            SysmelType baseTypeValue = Intrinsics.GetSingleton().DynamicType;
            if(baseType != null)
                baseTypeValue = (SysmelType)baseType.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().TypeUniverse0);

            EnumType enumType = new EnumType(sourcePosition, name, baseTypeValue);
            if(name != null)
                environment.SetSymbolBinding(name, enumType);

            Value enumValues = values.AnalyzeAndEvaluateInEnvironmentWithEnumBaseType(environment, baseTypeValue);
            if (!enumValues.IsImmutableDictionaryValue())
                throw new SysmelException(sourcePosition, "Expected a dictionary with the enum values.");

            ImmutableDictionaryValue enumSourceValues = (ImmutableDictionaryValue)enumValues;
            Value nextImplicitBaseTypeValue = baseTypeValue.MakeDefaultValue();

            foreach(AssociationValue association in enumSourceValues.elements)
            {
                string assocName = association.key.AsSymbol();
                Value assocValue = nextImplicitBaseTypeValue;
                if (!association.value.IsNilValue())
                    assocValue = association.value;

                nextImplicitBaseTypeValue = baseTypeValue.MakePlusOneValue(assocValue);
                EnumValue enumValue = new EnumValue(association.sourcePosition, enumType, assocName, assocValue);

                enumType.values.Add(enumValue);
                enumType.nameAndValues[assocName] = enumValue;
            }

            return enumType;
        }
    }
}
