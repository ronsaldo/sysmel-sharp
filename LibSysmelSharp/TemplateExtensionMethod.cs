using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class TemplateExtensionMethod
    {
        public ParseTreeMethodNode methodNode;
        public Environment environment;

        public TemplateExtensionMethod(ParseTreeMethodNode methodNode, Environment environment)
        {
            this.methodNode = methodNode;
            this.environment = environment;
        }
    }
}
