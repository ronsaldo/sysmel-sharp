using LibSysmelSharp;

namespace SysmelSharp
{
    internal class Program
    {
        static void printHelp()
        {
            Console.WriteLine("SysmelSharp <input files>");
        }

        static void printVersion()
        {
            Console.WriteLine("SysmelSharp 0.1");
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if(arg.StartsWith("-"))
                {
                    if(arg == "-help")
                    {
                        printHelp();
                        return;
                    }
                    else if (arg == "-version")
                    {
                        printVersion();
                        return;
                    }
                    else if(arg == "-eval")
                    {
                        SysmelFrontEnd.EvaluateString(args[++i]);
                    }
                }
                else
                {
                    SysmelFrontEnd.EvaluateFileNamed(args[i]);
                }
            }

            Intrinsics.GetSingleton().RunPendingAnalysis();
        }
    }
}
