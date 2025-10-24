using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeVariableDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode? nameExpression;
        public ParseTreeNode? typeExpression;
        public ParseTreeNode? initialValueExpression;
        public bool isMutable;

        public ParseTreeVariableDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode? nameExpression, ParseTreeNode? typeExpression, ParseTreeNode initialValueExpression, bool isMutable) : base(sourcePosition)
        {
            this.nameExpression = nameExpression;
            this.typeExpression = typeExpression;
            this.initialValueExpression = initialValueExpression;
            this.isMutable = isMutable;
        }

        public override bool IsParseTreeVariableDefinitionNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string? nameSymbolValue = null;
            if (nameExpression != null)
            {
                Value nameValue = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType);
                nameSymbolValue = nameValue.AsSymbol();
            }

            SysmelType? typeValue = null;
            if (typeExpression != null)
                typeValue = (SysmelType)typeExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);

            Value initialValue = VoidValue.GetSingleton();
            if (initialValueExpression != null)
                initialValue = initialValueExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, typeValue);

            if (isMutable)
            {
                SysmelType valueType = typeValue == null ? initialValue.GetSysmelType() : typeValue;

                MutableValueBox boxValue = new MutableValueBox(sourcePosition, initialValue, valueType.MutableBox());
                ReferenceLikeValue referenceValue = new ReferenceLikeValue(sourcePosition, boxValue, 0, valueType.Ref());
                if (nameSymbolValue != null)
                    environment.SetSymbolBinding(nameSymbolValue, referenceValue);
                return referenceValue;
            }
            else
            {
                if (nameSymbolValue != null)
                    environment.SetSymbolBinding(nameSymbolValue, initialValue);
                return initialValue;
            }
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            string? nameSymbolValue = null;
            if (nameExpression != null)
            {
                Value nameValue = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(builder.environment, Intrinsics.GetSingleton().SymbolType);
                nameSymbolValue = nameValue.AsSymbol();
            }

            SysmelType? typeValue = null;
            if (typeExpression != null)
                typeValue = (SysmelType)typeExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(builder.environment);

            HIRValue initialValue = builder.GetVoidLiteral(sourcePosition);
            if (initialValueExpression != null)
                initialValue = initialValueExpression.AnalyzeAndBuildWithExpectedType(builder, typeValue);

            if (isMutable)
            {
                Debug.Assert(builder.allocaBuilder != null);
                SysmelType valueType = typeValue == null ? initialValue.GetSysmelType() : typeValue;
                HIRAlloca alloca = builder.allocaBuilder.Alloca(sourcePosition, valueType, valueType.Ref());
                builder.StorePointer(sourcePosition, alloca, initialValue);

                if (nameSymbolValue != null)
                    builder.environment.SetSymbolBinding(nameSymbolValue, alloca);
                return alloca;
            }
            else
            {
                if (nameSymbolValue != null)
                    builder.environment.SetSymbolBinding(nameSymbolValue, initialValue);
                return initialValue;
            }
        }
    }
}