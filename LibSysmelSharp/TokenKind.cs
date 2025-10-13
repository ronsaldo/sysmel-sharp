namespace LibSysmelSharp
{
    public enum TokenKind
    {
        NullToken,
        EndOfSource,
        Error,

        Character,
        Float,
        Identifier,
        Integer,
        Keyword,
        MultiKeyword,
        Operator,
        Bar,
        LessThan,
        GreaterThan,
        String,
        Symbol,

        LeftParent,
        RightParent,
        LeftBracket,
        RightBracket,
        LeftCurlyBracket,
        RightCurlyBracket,


        Colon,
        ColonColon,
        Assignment,
        Semicolon,
        Comma,
        Dot,
        Ellipsis,
        Quote,
        QuasiQuote,
        QuasiUnquote,
        Splice,

        ByteArrayStart,
        ImmutableDictionaryStart,
        LiteralArrayStart,

        MethodStart
    }
}
