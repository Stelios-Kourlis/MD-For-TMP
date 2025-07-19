using System.Collections.Generic;
using System.Linq;

internal static class MarkdownConvertionListExtensions
{
    /// <summary>
    /// Will try to get the last symbol in the list.
    /// </summary>
    /// <param name="symbol">The latest symbol if it exists</param>
    /// <returns><c>True</c>  if a symbol existed in the list, else <c>False</c> </returns>
    public static bool TryLast(this List<MarkupSymbol> list, out MarkupSymbol symbol)
    {
        symbol = list.Count > 0 ? list.Last() : MarkupSymbol.Empty;
        return list.Count > 0;
    }

    /// <summary>
    /// Will add the new symbol to the list.
    /// <para>If the last symbol is the same type, it will instead update the last one as long as its length is less than <paramref name="maxRepeated"/>.</para>
    /// <para><c>Example:</c> Last Symbol is <c>##</c> and the new symbol is <c>#</c>, 
    /// it will be joined the last one to form <c>###</c> since the max repetions of <c>#</c> is 6, <c>False</c> will be returned.</para>
    /// <para><c>Example 2:</c> Last Symbol is <c>***</c> and the new symbol is <c>*</c>, 
    /// it will be added to the list as a new Symbol and wont be joined since the max repetions of <c>*</c> is 3, <c>True</c> will be returned.</para>
    /// <remarks><c>False</c> will also be returned if no <paramref name="maxRepeated"/> value is passed since a default of 1 means the symbol will never be joined.</remarks>
    /// </summary>
    /// <param name="symbol">The latest markup symbol</param>
    /// <param name="maxRepeated">The maximum times this symbol can be repeated at once</param>
    /// <returns><c>True</c> if a new symbol was added, <c>False</c> if it was instead joined with the last one</returns>
    public static bool AddOrJoin(this List<MarkupSymbol> list, MarkupSymbol symbol, int maxRepeated = 1)
    {
        bool OpenerOfSameType;
        if (list.TryLast(out MarkupSymbol last) && last.Symbol.Contains(symbol.Symbol) && !last.IsReplaced && last.Length < maxRepeated)
        {
            // string log = $"Joined to list: {symbol.Symbol}, JoinedWith: {last.Symbol}";
            last.Increment();
            OpenerOfSameType = list.Any(s => s.Symbol == last.Symbol && s.IsOpener && s != last);
            last.IsOpener = !OpenerOfSameType;
            // ConvertionLogger.Instance.Log(ConvertionLogger.LogType.Addition, $"{log} isOpener: {last.IsOpener} new length: {list.Count}");
            return false;
        }

        OpenerOfSameType = list.Any(s => s.Symbol == symbol.Symbol && s.IsOpener);
        symbol.IsOpener = !OpenerOfSameType;
        list.Add(symbol);
        // ConvertionLogger.Instance.Log(ConvertionLogger.LogType.Addition, $"Added to list: {symbol.Symbol}, On Top Of: {last?.Symbol ?? "-"} isOpener: {symbol.IsOpener} new length: {list.Count}");
        return last.Symbol.Contains(symbol.Symbol) && !last.IsReplaced && last.Length >= maxRepeated;
    }

    /// <summary>
    /// In a list of string, append <paramref name="text"/> to the last string in the list.
    /// <para>If the list is empty, add it the the list instead</para>
    /// </summary>
    /// <param name="text">The text to append</param>
    public static void AppendToLatestStringInList(this List<string> list, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (list.Count > 0)
        {
            string last = list.Last();
            last += text;
            list[^1] = last;
        }
        else
        {
            list.Add(text);
        }
    }

    /// <summary>
    /// In a list of string, preend <paramref name="text"/> to the last string in the list. (Add it to the start of the last string)
    /// <para>If the list is empty, add it the the list instead</para>
    /// </summary>
    /// <param name="text">The text to append</param>
    public static void PreendToLatestStringInList(this List<string> list, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (list.Count > 0)
        {
            string last = list.Last();
            last = text + last;
            list[^1] = last;
        }
        else
        {
            list.Add(text);
        }
    }

    /// <summary>
    /// In a list of string, append <paramref name="character"/> to the last string in the list.
    /// <para>If the list is empty, add it the the list instead</para>
    /// </summary>
    /// <param name="character">The character to append</param>
    public static void AppendToLatestStringInList(this List<string> list, char character) => AppendToLatestStringInList(list, character.ToString());

    /// <summary>
    /// In a list of string, preend <paramref name="text"/> to the last string in the list. (Add it to the start of the last string).
    /// <para>If the list is empty, add it the the list instead</para>
    /// </summary>
    /// <param name="character">The character to append</param>
    public static void PreendToLatestStringInList(this List<string> list, char character) => PreendToLatestStringInList(list, character.ToString());

    public static bool LastLineIsOnlyOneTypeOfCharacter(this List<string> list, char character)
    {
        if (list.Count == 0) return false;
        string lastLine = list.Last();
        return lastLine.Count(c => c == character) == lastLine.Length;
    }
}
