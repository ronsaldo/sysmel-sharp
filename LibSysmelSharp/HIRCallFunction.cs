using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRCallFunction : HIRInstruction
    {
        public HIRValue function;
        public HIRValue[] callArguments;

        public HIRCallFunction(SourcePosition? sourcePosition, HIRValue function, HIRValue[] callArguments, SysmelType resultType)
            : base(sourcePosition, resultType, null)
        {
            this.function = function;
            this.callArguments = callArguments;
        }

        public override bool IsCallFunctionInstruction()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.Write("  " + ToString() + " := call " + function.ToString());
            foreach (HIRValue value in callArguments)
                Console.Write(", " + value.ToString());
            Console.WriteLine();
        }

        public bool HasOnlyConstantLiteralValues()
        {
            foreach(HIRValue argument in callArguments)
            {
                if (!argument.IsConstantLiteralValue())
                    return false;
            }
            return true;
        }
        public HIRValue SimplifyWithBuilder(HIRBuilder builder)
        {
            if (!function.IsCompileTimeFunction() || !HasOnlyConstantLiteralValues())
                return this;

            int argumentCount = callArguments.Length;
            Value[] compileTimeArguments = new Value[argumentCount];
            for(int i = 0; i < argumentCount; ++i)
            {
                HIRConstantLiteralValue argumentValue = (HIRConstantLiteralValue)callArguments[i];
                compileTimeArguments[i] = argumentValue.literal;
            }

            Value evaluationResult = function.EvaluateWithArguments(compileTimeArguments);
            return evaluationResult.AnalyzeAndBuild(builder);
        }
    }
}
