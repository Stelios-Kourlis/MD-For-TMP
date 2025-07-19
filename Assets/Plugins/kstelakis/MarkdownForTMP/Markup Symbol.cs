internal class MarkupSymbol
{
    public string Symbol { get; set; }
    public bool IsReplaced { get; set; }
    public bool IsOpener { get; set; }
    public bool IsAtLineStart { get; set; }
    public bool HasSpaceAfter { get; set; }
    public int Length => Symbol.Length;
    public static MarkupSymbol Empty => new(string.Empty, false);
    public bool IsEmpty => Symbol == string.Empty;
    public bool IsNewLine => Symbol == "\n";

    public MarkupSymbol(string symbol, bool isAtLineStart = false)
    {
        Symbol = symbol;
        IsReplaced = false;
        IsAtLineStart = isAtLineStart;
    }

    public void Increment()
    {
        if (IsEmpty) return;
        Symbol += Symbol[0];
    }

}