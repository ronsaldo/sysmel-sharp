using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeVariableDefinitionNode : ParseTreeNode
    {
        public ParseTreeNode nameExpression;
        public ParseTreeNode? typeExpression;
        public ParseTreeNode? initialValueExpression;
        public bool isMutable;
        
        public ParseTreeVariableDefinitionNode(SourcePosition? sourcePosition, ParseTreeNode nameExpression, ParseTreeNode? typeExpression, ParseTreeNode initialValueExpression, bool isMutable) : base(sourcePosition)
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
            Value nameValue = nameExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().SymbolType);
            string nameSymbolValue = nameValue.AsSymbol();
            SysmelType? typeValue = null;
            if (typeExpression != null)
                typeValue = (SysmelType)typeExpression.AnalyzeAndEvaluateInEnvironmentWithDecayedType(environment);

            Value initialValue = VoidValue.GetSingleton();
            if (initialValueExpression != null)
                initialValue = initialValueExpression.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, typeValue);

            if (isMutable)
            {
                SysmelType valueType = initialValue.GetSysmelType();
                MutableValueBox boxValue = new MutableValueBox(sourcePosition, initialValue, valueType.MutableBox());
                ReferenceLikeValue referenceValue = new ReferenceLikeValue(sourcePosition, boxValue, 0, valueType.Ref());
                environment.SetSymbolBinding(nameSymbolValue, referenceValue);
                return referenceValue;
            }
            else
            {
                environment.SetSymbolBinding(nameSymbolValue, initialValue);
                return initialValue;
            }
        }
    }
}
