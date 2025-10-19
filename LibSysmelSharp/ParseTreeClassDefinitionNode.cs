namespace LibSysmelSharp
{
    public class ParseTreeClassDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode? nameExpression;
        public ParseTreeNode? superclass;
        public ParseTreeNode? metafields;
        public ParseTreeNode? fields;

        public ParseTreeClassDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode? nameExpression, ParseTreeNode? superclass, ParseTreeNode? metafields, ParseTreeNode? fields) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.superclass = superclass;
            this.metafields = metafields;
            this.fields = fields;
        }

        public override bool IsParseTreeClassDefinitionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string? name = null;
            if (nameExpression != null)
                name = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType).AsSymbol();
            BehaviorType validSuperclass = Intrinsics.GetSingleton().ProtoObjectClassType;
            if(superclass != null)
            {
                Value superclassValue = superclass.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                if (!superclassValue.IsType() || !((SysmelType)superclassValue).IsClassType())
                    throw new SysmelException(sourcePosition, "Superclass must be a class.");

                validSuperclass = (BehaviorType)superclassValue;
            }

            MetaclassType metaClass = new MetaclassType(sourcePosition, (BehaviorType)validSuperclass.GetSysmelType(), []);
            ClassType clazz = new ClassType(sourcePosition, name, validSuperclass, metaClass, []);
            if(name != null)
                environment.SetSymbolBinding(name, clazz);
            
            // Meta fields
            if(metafields != null)
            {
                Value fieldValues = metafields.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                if (!fieldValues.IsImmutableDictionaryValue())
                    throw new SysmelException(sourcePosition, "Expected a dictionary with the class meta fields.");

                ImmutableDictionaryValue fieldValuesDict = (ImmutableDictionaryValue)fieldValues;
                FieldDefinition[] fields = fieldValuesDict.EvaluateAsFieldListInEnviroment(environment);
                metaClass.SetFields(fields);
            }

            // Fields
            if (fields != null)
            {
                Value fieldValues = this.fields.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);
                if (!fieldValues.IsImmutableDictionaryValue())
                    throw new SysmelException(sourcePosition, "Expected a dictionary with the class meta fields.");

                ImmutableDictionaryValue fieldValuesDict = (ImmutableDictionaryValue)fieldValues;
                FieldDefinition[] fields = fieldValuesDict.EvaluateAsFieldListInEnviroment(environment);
                clazz.SetFields(fields);
            }

            return clazz;
        }
    }
}
