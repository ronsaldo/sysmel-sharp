using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SysmelType : Value
    {
        public Value? owner;

        public SysmelType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public virtual SysmelType GetBaseType()
        {
            throw new NotSupportedException();
        }

        public virtual SysmelType? GetSuperType()
        {
            return null;
        }

        public override Value? LookupSelector(string selector)
        {
            return null;
        }

        public override FieldDefinition? LookupFieldName(string selector)
        {
            return null;
        }

        public override Value PerformWithArguments(string selector, Value[] arguments)
        {
            Value? foundMethod = LookupSelector(selector);
            if(foundMethod == null)
            {
                foundMethod = GetSysmelType().LookupSelector(selector);
                if (foundMethod == null)
                    throw new SysmelException(sourcePosition, "Message not understood: #" + selector);
            }

            Value[] receiverArgument = new Value[] { this };
            Value[] callArguments = receiverArgument.Concat(arguments).ToArray();
            return foundMethod.EvaluateWithArguments(callArguments);
        }
        
        public override bool IsType()
        {
            return true;
        }

        public virtual bool IsSatisfiedBy(SysmelType other)
        {
            return other.IsSubtypeOf(this);
        }

        public virtual bool IsSubtypeOf(SysmelType other)
        {
            if (this.Equals(other))
                return true;

            SysmelType? supertype = GetSuperType();
            if(supertype != null)
                return supertype.IsSubtypeOf(other);
            return false;
        }

        public override void SetOwner(Value anOwner)
        {
            owner = anOwner;
        }

        public virtual bool IsAssociationType()
        {
            return false;
        }

        public virtual bool IsImmutableDictionaryType()
        {
            return false;
        }
        public virtual bool IsFunctionType()
        {
            return false;
        }
        public virtual bool IsDependentFunctionType()
        {
            return false;
        }


        public virtual bool IsListType()
        {
            return false;
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

        public virtual bool IsVoidType()
        {
            return false;
        }

        public virtual bool IsTupleType()
        {
            return false;
        }

        public virtual bool IsSimpleFunctionType()
        {
            return false;
        }

        public virtual bool IsEnumType()
        {
            return false;
        }

        public virtual bool IsRecordType()
        {
            return false;
        }
        public virtual bool IsIntegerType()
        {
            return false;
        }
        
        public virtual bool IsBehaviorType()
        {
            return false;
        }
        public virtual bool IsClassType()
        {
            return false;
        }
        public virtual bool IsMetaclassType()
        {
            return false;
        }

        public virtual Value MakeDefaultValue()
        {
            throw new NotImplementedException();
        }
        public virtual Value MakePlusOneValue(Value sourceValue)
        {
            throw new NotImplementedException();
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
        public virtual ListType ListOf()
        {
            return Intrinsics.GetSingleton().GetOrCreateListOfType(this);
        }

        public virtual SimpleFunctionType MakeSimpleFunctionWithResultType(SysmelType resultType)
        {
            if(IsVoidType())
                return new SimpleFunctionType(sourcePosition, [], resultType);
            return new SimpleFunctionType(sourcePosition, [this], resultType);
        }
        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().TypeUniverse0;
        }
    }
}
