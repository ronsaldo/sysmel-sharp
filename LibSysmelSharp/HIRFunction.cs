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

        public override void DumpToConsole()
        {
            Console.WriteLine(ToString() + "{");
            foreach (HIRArgument arg in arguments)
                arg.DumpToConsole();

            foreach (HIRCapture arg in captures)
                arg.DumpToConsole();

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
