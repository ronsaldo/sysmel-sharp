using System.Diagnostics;

namespace LibSysmelSharp
{
    public class HIRFunction : HIRConstant
    {
        public string? name;
        public List<HIRArgument> arguments;
        public List<HIRCapture> captures;
        public HIRBasicBlock? firstBlock;
        public HIRBasicBlock? lastBlock;

        public List<HIRValue>? enumeratedInstructions;

        public HIRFunction(SourcePosition? sourcePosition, string?name, SysmelType type) : base(sourcePosition, type)
        {
            this.name = name;
            arguments = new List<HIRArgument>();
            captures = new List<HIRCapture>();
        }

        public void AddArgument(HIRArgument argument)
        {
            argument.index = this.arguments.Count;
            this.arguments.Add(argument);
        }

        public void AddCapture(HIRCapture capture)
        {
            capture.index = this.arguments.Count + this.captures.Count;
            this.captures.Add(capture);
        }

        public void AddBasicBlock(HIRBasicBlock block)
        {
            Debug.Assert(block.owner == null);
            Debug.Assert(block.previousBlock == null);
            Debug.Assert(block.nextBlock == null);
            block.owner = this;

            if (lastBlock != null)
            {
                lastBlock.nextBlock = block;
                block.previousBlock = lastBlock;
                lastBlock = block;
            }
            else
            {
                firstBlock = lastBlock = block;
            }
        }

        public override string ToString()
        {
            if (name == null)
                return "@<anon>";
            return "@" + name;
        }

        public List<HIRValue> EnumerateInstructions()
        {
            if (enumeratedInstructions != null)
                return enumeratedInstructions;

            enumeratedInstructions = new List<HIRValue>();
            foreach (HIRArgument arg in arguments)
            {
                arg.index = enumeratedInstructions.Count();
                enumeratedInstructions.Add(arg);
            }

            foreach (HIRCapture capture in captures)
            {
                capture.index = enumeratedInstructions.Count;
                enumeratedInstructions.Add(capture);
            }

            HIRBasicBlock? blockPosition = firstBlock;
            while(blockPosition != null)
            {
                blockPosition.index = enumeratedInstructions.Count;
                enumeratedInstructions.Add(blockPosition);

                HIRInstruction? instructionPosition = blockPosition.firstInstruction;
                while(instructionPosition != null)
                {
                    instructionPosition.index = enumeratedInstructions.Count;
                    enumeratedInstructions.Add(instructionPosition);

                    instructionPosition = instructionPosition.nextInstruction;
                }

                blockPosition = blockPosition.nextBlock;
            }

            return enumeratedInstructions;
        }

        public override void DumpToConsole()
        {
            EnumerateInstructions();
            Console.WriteLine(ToString() + "{");
            foreach (HIRArgument arg in arguments)
                arg.DumpToConsole();

            foreach (HIRCapture capture in captures)
                capture.DumpToConsole();

            HIRBasicBlock? blockPosition = firstBlock;
            while(blockPosition != null)
            {
                blockPosition.DumpToConsole();
                blockPosition = blockPosition.nextBlock;
            }
            Console.WriteLine("}");
        }
    }
}
