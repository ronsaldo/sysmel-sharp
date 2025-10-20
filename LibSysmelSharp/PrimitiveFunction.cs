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
        public Value? owner = null;

        public PrimitiveFunction(string name, bool isMacro, bool isCompileTime, SysmelType functionType, PrimitiveFunctionDelegate implementation) : base(null, name, isMacro, isCompileTime, functionType)
        {
            this.implementation = implementation;
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            return implementation(callArguments);
        }

        public override bool IsPrimitiveFunctionValue()
        {
            return true;
        }

        public override void SetOwner(Value anOwner)
        {
            this.owner = anOwner;
        }
    }
}
