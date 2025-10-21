using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRImportedFunction : HIRConstant
    {
        public AbstractFunction importedFunction;

        public HIRImportedFunction(SourcePosition? sourcePosition, AbstractFunction importedFunction) : base(sourcePosition, importedFunction.GetSysmelType())
        {
            this.importedFunction = importedFunction;
        }

        public override bool IsImportedFunction()
        {
            return true;
        }

        public override bool IsMacroFunction()
        {
            return importedFunction.IsMacroFunction();
        }

        public override string ToString()
        {
            return "ImportedFunction(" + importedFunction.ToString() + ")";
        }
        public override bool IsCompileTimeFunction()
        {
            return importedFunction.IsCompileTimeFunction();
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            return importedFunction.EvaluateWithArguments(callArguments);
        }

        public override Value GetValueInEvaluationContext(HIRFunctionActivationContext context)
        {
            return importedFunction;
        }
    }
}
