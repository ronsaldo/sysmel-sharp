namespace LibSysmelSharp
{
    public class SourceCode
    {
        public string directory = "";
        public string name = "";
        public string text = "";
        public static SourceCode ForString(string name, string text)
        {
            SourceCode sourceCode = new SourceCode();
            sourceCode.name = name;
            sourceCode.text = text;
            return sourceCode;
        }

        public static SourceCode ForFileNamed(string filename)
        {
            string text = File.ReadAllText(filename);
            string? dirname = Path.GetDirectoryName(filename);

            SourceCode sourceCode = new SourceCode();
            if(dirname != null)
                sourceCode.directory = dirname;
            sourceCode.name = Path.GetFileName(filename);
            sourceCode.text = text;
            return sourceCode;
        }

        public override string ToString()
        {
            if (directory != "")
                return directory + "/" + name;
            return name;
        }
    }
}
