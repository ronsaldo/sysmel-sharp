using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SourcePosition
    {
        public SourceCode? sourceCode;
        public int startIndex;
        public int endIndex;
        public int startLine;
        public int endLine;
        public int startColumn;
        public int endColumn;


        public string GetText()
        {
            return sourceCode.text.Substring(startIndex, endIndex - startIndex);
        }

    }
}
