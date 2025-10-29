using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ControlFlowEscapeValue : Value
    {
        ControlFlowEscapeValue() : base(null)
        {
        }

        private static ControlFlowEscapeValue? uniqueInstance;
        public static ControlFlowEscapeValue GetSingleton()
        {
            if (uniqueInstance == null)
                uniqueInstance = new ControlFlowEscapeValue();
            return uniqueInstance;
        }

        public override bool IsControlFlowEscapeValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().ControlFlowEscapeType;
        }

        public override string ToString()
        {
            return "cfescape";
        }
    }
}
