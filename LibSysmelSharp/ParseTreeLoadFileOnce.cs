using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ParseTreeLoadFileOnce : ParseTreeNode
    {
        public ParseTreeNode sourceFileName;

        public ParseTreeLoadFileOnce(SourcePosition? sourcePosition, ParseTreeNode sourceFileName) : base(sourcePosition)
        {
            this.sourceFileName = sourceFileName;
        }

        public override bool IsParseTreeLoadFileOnce()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string fileDir = ".";
            Value? fileDirValue = environment.LookupRecursively("__FileDir__");
            if (fileDirValue != null)
                fileDir = fileDirValue.AsString();
            
            string sourceFileNameValue = sourceFileName.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().StringType).AsString();
            string fileNameToLoad = Path.Combine(fileDir, sourceFileNameValue);

            return SysmelFrontEnd.EvaluateFileNamed(fileNameToLoad);
        }
    }
}
