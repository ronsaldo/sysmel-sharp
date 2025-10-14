using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class VoidValue : Value
    {
        VoidValue() : base(null)
        {
        }

        private static VoidValue? uniqueInstance;
        public static VoidValue GetSingleton()
        {
            if (uniqueInstance == null)
                uniqueInstance = new VoidValue();
            return uniqueInstance;
        }

        public override bool IsVoidValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().VoidType;
        }
    }
}
