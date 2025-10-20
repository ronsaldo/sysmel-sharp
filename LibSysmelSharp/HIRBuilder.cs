using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRBuilder
    {
        public Environment environment;
        public HIRFunction function;
        public HIRBasicBlock? basicBlock;
        public HIRBuilder? allocaBuilder;

        public HIRBuilder(Environment environment, HIRFunction function)
        {
            this.environment = environment;
            this.function = function;
        }

        public void AddInstruction(HIRInstruction instruction)
        {
            Debug.Assert(basicBlock != null);
            basicBlock.AddIntruction(instruction);
        }

        public HIRValue AddLiteral(SourcePosition? sourcePosition, Value value)
        {
            return new HIRConstantLiteralValue(sourcePosition, value);
        }

        public HIRAlloca Alloca(SourcePosition? sourcePosition, SysmelType valueType, SysmelType type)
        {
            HIRAlloca alloca = new HIRAlloca(sourcePosition, valueType, type);
            AddInstruction(alloca);
            return alloca;
        }

        public HIRBranch Branch(SourcePosition? sourcePosition, HIRBasicBlock destination)
        {
            HIRBranch branch = new HIRBranch(sourcePosition, destination);
            AddInstruction(branch);
            return branch;
        }

        public HIRSwitch Switch(SourcePosition? sourcePosition, HIRValue switchValue, HIRValue[] caseValues, HIRBasicBlock[] caseBlocks, HIRBasicBlock defaultDestination)
        {
            HIRSwitch switchBranch = new HIRSwitch(sourcePosition, switchValue, caseValues, caseBlocks, defaultDestination);
            AddInstruction(switchBranch);
            return switchBranch;
        }

        public HIRBuilder CopyWithBasicBlock(HIRBasicBlock newBasicBlock)
        {
            HIRBuilder copy = new HIRBuilder(environment, function);
            copy.basicBlock = newBasicBlock;
            copy.allocaBuilder = allocaBuilder;
            return copy;
        }

        public HIRConditionalBranch ConditionalBranch(SourcePosition? sourcePosition, HIRValue condition, HIRBasicBlock trueDestination, HIRBasicBlock falseDestination)
        {
            HIRConditionalBranch branch = new HIRConditionalBranch(sourcePosition, condition, trueDestination, falseDestination);
            AddInstruction(branch);
            return branch;
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

            HIRLoadInstruction loadInstruction = new HIRLoadInstruction(sourcePosition, storageType.GetBaseType(), storage);
            AddInstruction(loadInstruction);
            return loadInstruction;
        }
        public HIRStoreInstruction StorePointer(SourcePosition? sourcePosition, HIRValue storage, HIRValue valueToStore)
        {
            HIRStoreInstruction storeInstruction = new HIRStoreInstruction(sourcePosition, storage, valueToStore);
            AddInstruction(storeInstruction);
            return storeInstruction;
        }

        public HIRExtractFieldReference ExtractFieldReference(SourcePosition? sourcePosition, HIRValue aggregate, FieldDefinition fieldDefinition)
        {
            HIRExtractFieldReference instruction = new HIRExtractFieldReference(sourcePosition, aggregate, fieldDefinition);
            AddInstruction(instruction);
            return instruction;
        }

        public HIRBoxDynamic BoxDynamic(SourcePosition? sourcePosition, HIRValue valueToBox)
        {
            HIRBoxDynamic boxInstruction = new HIRBoxDynamic(sourcePosition, valueToBox);
            AddInstruction(boxInstruction);
            return boxInstruction;
        }

        public HIRUnboxDynamic UnboxDynamic(SourcePosition? sourcePosition, HIRValue boxedValue, SysmelType unboxedType)
        {
            HIRUnboxDynamic unboxInstruction = new HIRUnboxDynamic(sourcePosition, boxedValue, unboxedType);
            AddInstruction(unboxInstruction);
            return unboxInstruction;
        }

        public HIRValue  CallFunction(SourcePosition? sourcePosition, HIRValue function, HIRValue[] callArguments, SysmelType resultType)
        {
            HIRCallFunction callInstruction = new HIRCallFunction(sourcePosition, function, callArguments, resultType);
            HIRValue simplified = callInstruction.SimplifyWithBuilder(this);
            if(simplified == callInstruction)
                AddInstruction((HIRInstruction)simplified);
            return simplified;
        }

        public HIRReturnValue ReturnValue(SourcePosition? sourcePosition, HIRValue value)
        {
            HIRReturnValue instruction = new HIRReturnValue(sourcePosition, value);
            AddInstruction(instruction);
            return instruction;
        }

        public HIRReturnVoid ReturnVoid(SourcePosition? sourcePosition)
        {
            HIRReturnVoid instruction = new HIRReturnVoid(sourcePosition);
            AddInstruction(instruction);
            return instruction;
        }

        public HIRPhi Phi(SourcePosition? sourcePosition, SysmelType type, string? name)
        {
            HIRPhi phi = new HIRPhi(sourcePosition, type, name);
            AddInstruction(phi);
            return phi;
        }
        
        public HIRPhiSource PhiSource(SourcePosition? sourcePosition, HIRPhi destinationPhi, HIRValue sourceValue)
        {
            HIRPhiSource phiSource = new HIRPhiSource(sourcePosition, destinationPhi, sourceValue);
            AddInstruction(phiSource);
            return phiSource;
        }

        public bool IsLastTerminator()
        {
            return basicBlock != null && basicBlock.IsLastTerminator();
        }
    }
}
