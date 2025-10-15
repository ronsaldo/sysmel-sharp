using System.Collections.Generic;

namespace LibSysmelSharp
{
    public class LexicalEnvironment : Environment
    {
        public Environment parent;
        public Dictionary<string, Value> localSymbolTable;

        public LexicalEnvironment(Environment parent)
        {
            this.parent = parent;
            this.localSymbolTable = new Dictionary<string, Value> ();
        }

        public override void SetSymbolBinding(string symbol, Value binding)
        {
            localSymbolTable[symbol] = binding;
        }

        public override Value? LookupRecursively(string symbol)
        {
            if(localSymbolTable.ContainsKey(symbol))
                return localSymbolTable[symbol];

            return parent.LookupRecursively(symbol);
        }

    }
}
