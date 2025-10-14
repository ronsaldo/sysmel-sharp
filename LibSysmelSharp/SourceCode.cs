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

        public override string ToString()
        {
            if (directory != "")
                return directory + "/" + name;
            return name;
        }
    }
}
