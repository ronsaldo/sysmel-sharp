using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class FunctionalSignatureAnalysisEnvironment : Environment
    {
        public Environment parent;
        public Dictionary<string, Value> localSymbolTable;
        public List<ArgumentDefinitionValue> dependentArguments;
        public List<ArgumentDefinitionValue> argumentList;

        public FunctionalSignatureAnalysisEnvironment(Environment parent)
        {
            this.parent = parent;
            this.localSymbolTable = new Dictionary<string, Value>();
            this.dependentArguments = new List<ArgumentDefinitionValue>();
            this.argumentList = new List<ArgumentDefinitionValue>();
        }

        public int GetArgumentCount()
        {
            return argumentList.Count;
        }

        public void AddArgument(ArgumentDefinitionValue argument)
        {
            if (argument.name != null)
                this.localSymbolTable[argument.name] = argument;
            argumentList.Add(argument);
        }
        void MarkUsedArgumentDefinition(ArgumentDefinitionValue argument)
        {
            if (argument.hasDependentUser)
                return;

            argument.hasDependentUser = true;
            dependentArguments.Add(argument);
        }

        public override Value? LookupRecursively(string symbol)
        {
            if (localSymbolTable.ContainsKey(symbol))
            {
                Value localSymbol = localSymbolTable[symbol];
                if (localSymbol.IsArgumentDefinitionValue())
                    MarkUsedArgumentDefinition((ArgumentDefinitionValue)localSymbol);
                return localSymbol;
            }

            return parent.LookupRecursively(symbol);
        }

        public override Namespace? LookNamespace()
        {
            return parent.LookNamespace();
        }
    }
}
