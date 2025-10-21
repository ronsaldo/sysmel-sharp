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
        
        public List<HIRCapture> captures;
        public Dictionary<string, Value> captureDictionary;

        public FunctionAnalysisEnvironment(HIRFunction hirFunction, Environment parent)
        {
            this.parent = parent;
            this.localSymbolTable = new Dictionary<string, Value>();
            this.hirFunction = hirFunction;
            this.captures = new List<HIRCapture>();
            this.captureDictionary = new Dictionary<string, Value> ();
        }

        public override void SetSymbolBinding(string symbol, Value binding)
        {
            localSymbolTable[symbol] = binding;
        }

        public override Value? LookupRecursively(string symbol)
        {
            if (localSymbolTable.ContainsKey(symbol))
                return localSymbolTable[symbol];

            if (captureDictionary.ContainsKey(symbol))
                return captureDictionary[symbol];   

            if (receiverValue != null)
            {
                FieldDefinition? field = receiverValue.LookupFieldName(symbol);
                if (field != null)
                    return new ImplicitFieldAccess(null, receiverValue, field);
            }

            Value? parentValue = parent.LookupRecursively(symbol);
            if (parentValue == null)
                return null;

            if (!parentValue.IsCapturableValue())
                return parentValue;

            if (parentValue.IsImplicitFieldAccess())
            {
                throw new NotImplementedException();
            }

            HIRCapture newCapture = new HIRCapture(null, parentValue.GetSysmelType(), null);
            newCapture.sourceValue = (HIRValue)parentValue;

            captures.Add(newCapture);
            captureDictionary[symbol] = newCapture;
            hirFunction.AddCapture(newCapture);
            return newCapture;
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
