using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SourcePosition
    {
        public SourceCode sourceCode;
        public int startIndex;
        public int endIndex;
        public int startLine;
        public int endLine;
        public int startColumn;
        public int endColumn;

        public SourcePosition(SourceCode sourceCode)
        {
            this.sourceCode = sourceCode;
        }

        public string GetText()
        {
            return sourceCode.text.Substring(startIndex, endIndex - startIndex);
        }

        public SourcePosition To(SourcePosition endPosition)
        {
            SourcePosition newPosition = new SourcePosition(sourceCode);
            newPosition.startIndex  = startIndex;
            newPosition.endIndex    = endPosition.endIndex;
            newPosition.startLine   = startLine;
            newPosition.endLine     = endPosition.endLine;
            newPosition.startColumn = startColumn;
            newPosition.endColumn   = endPosition.endColumn;
            return newPosition;
        }

        public SourcePosition Until(SourcePosition endPosition)
        {
            return To(endPosition);
        }

    }
}
