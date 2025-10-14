using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class MacroContextValue : Value
    {
        public Environment environment;

        public MacroContextValue(SourcePosition? sourcePosition, Environment environment) : base(sourcePosition)
        {
            this.environment = environment;
        }

        public override bool isMacroContextValue()
        {
            return true;
        }
    }
}
