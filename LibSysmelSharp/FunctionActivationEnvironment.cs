using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    internal class FunctionActivationEnvironment : Environment
    {
        public Environment parent;
        public Dictionary<string, Value> localSymbolTable;
        public Value? receiverValue;


        public FunctionActivationEnvironment(Environment parent)
        {
            this.parent = parent;
            this.localSymbolTable = new Dictionary<string, Value>();
        }

        public override void SetSymbolBinding(string symbol, Value binding)
        {
            localSymbolTable[symbol] = binding;
            if (symbol == "self")
                receiverValue = binding;
        }

        public override Value? LookupRecursively(string symbol)
        {
            if (localSymbolTable.ContainsKey(symbol))
                return localSymbolTable[symbol];

            if (receiverValue != null)
            {
                FieldDefinition? field = receiverValue.LookupFieldName(symbol);
                if(field != null)
                {
                    ReferenceType fieldReferenceType = field.type.Ref();
                    ReferenceLikeValue fieldReference = new ReferenceLikeValue(null, receiverValue, field.index, fieldReferenceType);
                    return fieldReference;
                }
            }

            return parent.LookupRecursively(symbol);
        }

        public override Namespace? LookNamespace()
        {
            return parent.LookNamespace();
        }
    }
}
