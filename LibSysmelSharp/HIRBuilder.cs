using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRBuilder
    {
        public Environment? environment;
        public HIRBasicBlock? basicBlock;
        public HIRBuilder? allocaBuilder;

        public HIRValue AddLiteral(SourcePosition? sourcePosition, Value value)
        {
            return new HIRConstantLiteralValue(sourcePosition, value);
        }

        public HIRValue GetVoidLiteral(SourcePosition? sourcePosition)
        {
            return AddLiteral(sourcePosition, VoidValue.GetSingleton());
        }

        public HIRLoadInstruction LoadPointer(SourcePosition? sourcePosition, HIRValue storage)
        {
            SysmelType storageType = storage.GetSysmelType();
            if (!storageType.IsPointerLikeType())
                throw new SysmelException(sourcePosition, "Expected a value with a pointer like type.");

            return new HIRLoadInstruction(sourcePosition, storageType.GetBaseType(), storage);
        }

        public HIRBoxDynamic BoxDynamic(SourcePosition? sourcePosition, HIRValue valueToBox)
        {
            return new HIRBoxDynamic(sourcePosition, valueToBox);
        }

        public HIRUnboxDynamic UnboxDynamic(SourcePosition? sourcePosition, HIRValue boxedValue, SysmelType unboxedType)
        {
            return new HIRUnboxDynamic(sourcePosition, boxedValue, unboxedType);
        }
    }
}
