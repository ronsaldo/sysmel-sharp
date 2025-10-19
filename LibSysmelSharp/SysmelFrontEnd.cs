using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SysmelFrontEnd
    {
        public static Value EvaluateSourceCode(SourceCode sourceCode)
        {
            List<Token> tokenList = new Scanner().ScanSourceCode(sourceCode);
            ParseTreeNode parseTree = new Parser().ParseTokenList(sourceCode, tokenList);
            Environment environment = new LexicalEnvironment(new NamespaceEnvironment(Intrinsics.GetSingleton().globalNamespace, new EmptyEnvironment()));
            environment.SetSymbolBinding("__FileDir__", new StringValue(null, sourceCode.directory, Intrinsics.GetSingleton().StringType));
            environment.SetSymbolBinding("__FileName__", new StringValue(null, sourceCode.name, Intrinsics.GetSingleton().StringType));

            return parseTree.AnalyzeAndEvaluateInEnvironment(environment);
        }

        public static Value EvaluateString(string script)
        {
            SourceCode sourceCode = SourceCode.ForString("cli", script);
            return EvaluateSourceCode(sourceCode);
        }
        public static Value EvaluateFileNamed(string filename)
        {
            SourceCode sourceCode = SourceCode.ForFileNamed(filename);
            return EvaluateSourceCode(sourceCode);
        }
    }
}
