using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public delegate Value PrimitiveFunctionDelegate(Value[] callArguments);

    public class PrimitiveFunction : AbstractFunction
    {
        public PrimitiveFunctionDelegate implementation;

        public PrimitiveFunction(string name, bool isMacro, SysmelType functionType, PrimitiveFunctionDelegate implementation) : base(null, name, isMacro, functionType)
        {
            this.implementation = implementation;
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            return implementation(callArguments);
        }

    }
}
