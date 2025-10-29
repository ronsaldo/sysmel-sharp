using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class NominalType : SimpleType
    {
        public string name;

        public NominalType(SourcePosition? sourcePosition, string name) : base(sourcePosition)
        {
            this.name = name;
        }

        public override string? GetName()
        {
            return name;
        }

        public override bool IsVoidType()
        {
            return this.Equals(Intrinsics.GetSingleton().VoidType);
        }

        public override bool IsControlFlowEscapeType()
        {
            return this.Equals(Intrinsics.GetSingleton().ControlFlowEscapeType);
        }

        public override bool IsDynamicType()
        {
            return this.Equals(Intrinsics.GetSingleton().DynamicType);
        }

        public override bool IsUndefinedObjectType()
        {
            return this.Equals(Intrinsics.GetSingleton().UndefinedObjectType);
        }

        public override string ToString()
        {
            return name;
        }
        public override Value MakeDefaultValue()
        {
            return NilValue.GetSingleton();
        }

        public virtual long NormalizeInteger(long value)
        {
            return value;
        }
        public virtual double NormalizeFloat(double value)
        {
            return value;
        }
    }
}
