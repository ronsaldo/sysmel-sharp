using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class Token
    {
        public TokenKind kind = TokenKind.NullToken;
        public SourcePosition sourcePosition = new SourcePosition();
        public string errorMessage = "";

        public string GetText()
        {
            return sourcePosition.GetText();
        }
    }
}
