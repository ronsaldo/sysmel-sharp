using LibSysmelSharp;

namespace SysmelSharp
{
    internal class Program
    {
        static void printHelp()
        {
            Console.WriteLine("SysmelSharp <input files>");
        }

        static void printVersion()
        {
            Console.WriteLine("SysmelSharp 0.1");
        }

        static Value EvaluateSourceCode(SourceCode sourceCode)
        {
            List<Token> tokenList = new Scanner().ScanSourceCode(sourceCode);
            ParseTreeNode parseTree = new Parser().ParseTokenList(sourceCode, tokenList);
            LibSysmelSharp.Environment environment = new LexicalEnvironment(new IntrinsicsEnvironment(new EmptyEnvironment()));
            return parseTree.AnalyzeAndEvaluateInEnvironment(environment);
        }

        static void evaluateString(string script)
        {
            SourceCode sourceCode = SourceCode.ForString("cli", script);
            Value evalResult = EvaluateSourceCode(sourceCode);
            Console.WriteLine(evalResult.ToString());
        }
        static void evaluateFileNamed(string filename)
        {
            Console.WriteLine("TODO: evaluate file named " + filename);
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if(arg.StartsWith("-"))
                {
                    if(arg == "-help")
                    {
                        printHelp();
                        return;
                    }
                    else if (arg == "-version")
                    {
                        printVersion();
                        return;
                    }
                    else if(arg == "-eval")
                    {
                        evaluateString(args[++i]);
                    }
                }
                else
                {
                    evaluateFileNamed(args[i]);
                }
            }
            Console.WriteLine("Hello, World!");
        }
    }
}
