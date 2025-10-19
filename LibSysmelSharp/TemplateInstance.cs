using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class TemplateInstance
    {
        public MemoizedFunctionKey instanceArguments;
        public Value instanceValue;

        public TemplateInstance(MemoizedFunctionKey instanceArguments, Value instanceValue  )
        {
            this.instanceArguments = instanceArguments;
            this.instanceValue = instanceValue;
        }
    }
}
