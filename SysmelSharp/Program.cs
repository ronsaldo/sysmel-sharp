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

        static bool EvaluateString(string s)
        {
            try
            {
                SysmelFrontEnd.EvaluateString(s);
                return true;
            }
            catch(SysmelException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        static bool EvaluateFileNamed(string fileName)
        {
            try
            {
                SysmelFrontEnd.EvaluateFileNamed(fileName);
                return true;
            }
            catch (SysmelException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        static bool RunPendingAnalysis()
        {
            try
            {
                Intrinsics.GetSingleton().RunPendingAnalysis();
                return true;
            }
            catch (SysmelException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }


        static int Main(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if(arg.StartsWith("-"))
                {
                    if(arg == "-help")
                    {
                        printHelp();
                        return 0;
                    }
                    else if (arg == "-version")
                    {
                        printVersion();
                        return 0;
                    }
                    else if(arg == "-eval")
                    {
                        if (!EvaluateString(args[++i]))
                            return 1;
                    }
                }
                else
                {
                    if (!EvaluateFileNamed(args[i]))
                        return 1;
                }
            }

            if (!RunPendingAnalysis())
                return 1;
            return 0;
        }
    }
}
