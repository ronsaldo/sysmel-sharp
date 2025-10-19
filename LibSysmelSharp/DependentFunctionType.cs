using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class DependentFunctionType : AbstractFunctionType
    {
        public string? selector;
        public ArgumentDefinitionValue[] arguments;
        public Value resultTypeExpression;
        public bool hasDependentArguments;
        public bool isForAll;

        public DependentFunctionType(SourcePosition? sourcePosition, string? selector, ArgumentDefinitionValue[] arguments, Value resultTypeExpression, bool isForAll) : base(sourcePosition)
        {
            this.selector = selector;
            this.arguments = arguments;
            this.resultTypeExpression = resultTypeExpression;
            this.hasDependentArguments = false;
            foreach(ArgumentDefinitionValue arg in arguments)
            {
                if (arg.hasDependentUser)
                    this.hasDependentArguments = true;
            }
            this.isForAll = isForAll;
        }

        public override bool IsDependentFunctionType()
        {
            return true;
        }

        public SysmelType TryToSimplifyFunctionType()
        {
            if (hasDependentArguments && resultTypeExpression.IsType())
                return this;

            int argumentCount = arguments.Length;
            SysmelType[] argumentTypes = new SysmelType[argumentCount];
            for (int i = 0; i < argumentCount; ++i)
                argumentTypes[i] = arguments[i].type;
            return new SimpleFunctionType(sourcePosition, argumentTypes, (SysmelType)resultTypeExpression);
        }

    }
}
