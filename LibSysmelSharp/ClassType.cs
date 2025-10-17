using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ClassType : BehaviorType
    {
        public string name;
        public MetaclassType type;

        public ClassType(SourcePosition? sourcePosition, string name, BehaviorType? superclass, MetaclassType type, FieldDefinition[] fields) :
            base(sourcePosition, superclass, fields)
        {
            this.name = name;
            this.type = type;
        }

        public override bool IsClassType()
        {
            return true;
        }
    }
}
