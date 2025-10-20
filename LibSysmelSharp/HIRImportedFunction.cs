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
    }
}
