using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class Namespace : Value
    {
        public string name;
        public Namespace? parent;
        public Dictionary<string, Value> symbolDictionary;

        public Namespace(SourcePosition? sourcePosition, string name, Namespace? parent) : base(sourcePosition)
        {
            this.name = name;
            this.parent = parent;
            this.symbolDictionary = new Dictionary<string, Value>();
        }

        public override bool IsNamespace()
        {
            return true;
        }

        public void SetSymbolBinding(string symbol, Value binding)
        {
            symbolDictionary[symbol] = binding;
        }

        public Value? LookSymbol(string symbol)
        {
            if(symbolDictionary.ContainsKey(symbol))
                return symbolDictionary[symbol];
            return null;
        }

        public Namespace GetOrCreateChildNamespace(SourcePosition? sourcePosition, string childName)
        {
            if(symbolDictionary.ContainsKey(childName))
            {
                Value child = symbolDictionary[childName];
                if (!child.IsNamespace())
                    throw new SysmelException(sourcePosition, "Namespace name #" + childName + " clashes");
                return (Namespace)child;
            }

            Namespace newNamespace = new Namespace(sourcePosition, childName, this);
            SetSymbolBinding(childName, newNamespace);
            return newNamespace;
        }
    }
}
