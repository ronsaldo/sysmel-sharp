using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class NilValue : Value
    {
        NilValue() : base(null)
        {
        }

        private static NilValue? uniqueInstance;
        public static NilValue GetSingleton()
        {
            if(uniqueInstance == null)
                uniqueInstance = new NilValue();
            return uniqueInstance;
        }

        public override bool IsNilValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().UndefinedObjectType;
        }
    }
}
