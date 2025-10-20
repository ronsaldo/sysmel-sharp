using System.Collections.Generic;

namespace LibSysmelSharp
{
    public class FunctionAnalysisEnvironment : Environment
    {
        public Environment parent;
        public Dictionary<string, Value> localSymbolTable;
        public HIRFunction hirFunction;
        public SysmelType? returnTypeValue;
        public HIRValue? receiverValue;

        public FunctionAnalysisEnvironment(HIRFunction hirFunction, Environment parent)
        {
            this.parent = parent;
            this.localSymbolTable = new Dictionary<string, Value>();
            this.hirFunction = hirFunction;
        }

        public override void SetSymbolBinding(string symbol, Value binding)
        {
            localSymbolTable[symbol] = binding;
        }

        public override Value? LookupRecursively(string symbol)
        {
            if (localSymbolTable.ContainsKey(symbol))
                return localSymbolTable[symbol];

            if(receiverValue != null)
            {
                FieldDefinition? field = receiverValue.LookupFieldName(symbol);
                if (field != null)
                    return new ImplicitFieldAccess(null, receiverValue, field);
            }

            return parent.LookupRecursively(symbol);
        }
        public override Namespace? LookNamespace()
        {
            return parent.LookNamespace();
        }
        public override Value? LookEntityOwner()
        {
            return parent.LookEntityOwner();
        }

    }
}
