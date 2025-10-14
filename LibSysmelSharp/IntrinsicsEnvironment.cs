using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class IntrinsicsEnvironment : Environment
    {
        public Environment parent;

        public IntrinsicsEnvironment(Environment parent)
        {
            this.parent = parent;
        }

        public override Value? LookupRecursively(string symbol)
        {
            Value? intrinsicValue = Intrinsics.GetSingleton().LookIntrinsicGlobal(symbol);
            if (intrinsicValue != null)
                return intrinsicValue;

            return parent.LookupRecursively(symbol);
        }
    }

}
