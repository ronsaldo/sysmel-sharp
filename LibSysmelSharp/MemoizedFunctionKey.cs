using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class MemoizedFunctionKey
    {
        public Value[] arguments;

        public MemoizedFunctionKey(Value[] arguments)
        {
            this.arguments = arguments;
        }

        public override int GetHashCode()
        {
            int hashCode = arguments.Length.GetHashCode();
            foreach(Value value in arguments)
                hashCode ^= value.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not MemoizedFunctionKey) return false;
            MemoizedFunctionKey other = (MemoizedFunctionKey)obj;

            int argumentCount = arguments.Length;
            if (argumentCount != other.arguments.Length)
                return false;

            for(int i = 0; i < argumentCount; ++i)
            {
                if (!arguments[i].Equals(other.arguments[i]))
                    return false;
            }

            return true;
        }
    }
}
