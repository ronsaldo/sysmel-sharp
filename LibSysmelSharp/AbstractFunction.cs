using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibSysmelSharp
{
    public class AbstractFunction : Value
    {
        public SysmelType functionType;
        public string? name;
        public bool isMacro;
        public bool isCompileTime;

        public AbstractFunction(SourcePosition? sourcePosition, string? name, bool isMacro, bool isCompileTime, SysmelType functionType) : base(sourcePosition)
        {
            this.functionType = functionType;
            this.name = name;
            this.isMacro = isMacro;
            this.isCompileTime = isCompileTime;
        }

        public override SysmelType GetSysmelType()
        {
            return functionType;
        }

        public override bool IsMacroFunction()
        {
            return isMacro;
        }
        public override bool IsCompileTimeFunction()
        {
            return isCompileTime;
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return new HIRImportedFunction(sourcePosition, this);
        }

        public override HIRValue AnalyzeAndBuildIdentifierReference(ParseTreeIdentifierReferenceNode node, HIRBuilder builder)
        {
            return new HIRImportedFunction(node.sourcePosition, this);
        }

        public override string ToString()
        {
            if (name != null)
                return name;
            return base.ToString();
        }
    }
}
