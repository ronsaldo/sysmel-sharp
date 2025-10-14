using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class Token
    {
        public TokenKind kind;
        public SourcePosition sourcePosition;
        public string errorMessage;

        public Token(TokenKind kind, SourcePosition sourcePosition, string errorMessage = "")
        {
            this.kind = kind;
            this.sourcePosition = sourcePosition;
            this.errorMessage = errorMessage;
        }

        public string GetText()
        {
            return sourcePosition.GetText();
        }
    }
}
