using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRExtractFieldReference : HIRInstruction
    {
        public HIRValue aggregate;
        public FieldDefinition fieldDefinition;

        public HIRExtractFieldReference(SourcePosition? sourcePosition, HIRValue aggregate, FieldDefinition fieldDefinition)
            : base(sourcePosition, fieldDefinition.type.Ref(), null)
        {
            this.aggregate = aggregate;
            this.fieldDefinition = fieldDefinition;
        }

        public override bool IsExtractFieldReference()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := from " + aggregate.ToString() + " extractFieldRef " + fieldDefinition.ToString());
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            Value aggregateValue = aggregate.GetValueInEvaluationContext(context);

            ReferenceLikeValue fieldReference = new ReferenceLikeValue(sourcePosition, aggregateValue, fieldDefinition.index, GetSysmelType());
            context.SetCurrentInstructionValue(fieldReference);
        }
    }
}
