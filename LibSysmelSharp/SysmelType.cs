using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SysmelType : Value
    {
        public SysmelType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public virtual SysmelType GetBaseType()
        {
            throw new NotSupportedException();
        }

        public override Value? LookupSelector(string selector)
        {
            return null;
        }

        public virtual string? GetName()
        {
            return null;
        }
        
        public override bool IsType()
        {
            return true;
        }

        public virtual bool IsSatisfiedBy(SysmelType other)
        {
            return this == other;
        }

        public virtual bool IsMutableBoxType()
        {
            return false;
        }

        public virtual bool IsPointerLikeType()
        {
            return false;
        }

        public virtual bool IsPointerType()
        {
            return false;
        }

        public virtual bool IsReferenceType()
        {
            return false;
        }

        public virtual MutableBoxType MutableBox()
        {
            return Intrinsics.GetSingleton().GetOrCreateMutableBoxType(this);
        }

        public virtual PointerType Pointer()
        {
            return Intrinsics.GetSingleton().GetOrCreatePointerType(this);
        }

        public virtual ReferenceType Ref()
        {
            return Intrinsics.GetSingleton().GetOrCreateReferenceType(this);
        }
    }
}
