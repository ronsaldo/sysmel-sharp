namespace LibSysmelSharp
{
    public class Intrinsics
    {
        static private Intrinsics? uniqueInstance;
        private Dictionary<string, Value> globalIntrinsicDictionary = new Dictionary<string, Value>();

        public UniverseType TypeUniverse0;
        public NominalType CharacterType;
        public NominalType IntegerType;
        public NominalType FloatType;
        public NominalType StringType;
        public NominalType SymbolType;

        public NominalType ParseTreeNodeType;

        public Value? LookIntrinsicGlobal(string symbol)
        {
            if (!globalIntrinsicDictionary.ContainsKey(symbol))
                return null;
            return globalIntrinsicDictionary[symbol];
        }

        NominalType CreateNominalType(string name)
        {
            NominalType type = new NominalType(null, name);
            globalIntrinsicDictionary[name] = type;
            return type;
        }

        internal Intrinsics()
        {
            TypeUniverse0 = new UniverseType(null, 0);
            globalIntrinsicDictionary["Type"] = TypeUniverse0;

            CharacterType = CreateNominalType("Character");
            IntegerType = CreateNominalType("Integer");
            FloatType = CreateNominalType("Float");
            StringType= CreateNominalType("String");
            SymbolType = CreateNominalType("Symbol");

            ParseTreeNodeType = CreateNominalType("ParseTreeNode");
        }

        public static void Reset()
        {
            uniqueInstance = null;
        }

        public static Intrinsics GetSingleton()
        {
            if(uniqueInstance == null)
                uniqueInstance = new Intrinsics();

            return uniqueInstance;
        }

    }
}
