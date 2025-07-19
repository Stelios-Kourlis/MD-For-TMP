using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

// namespace kstelakis.MarkdownForTMP
// {

[RequireComponent(typeof(TMP_Text))]
public class MarkdownToTMPConverter : MonoBehaviour, IPointerClickHandler
{
    private string MarkdownText => '\n' + gameObject.GetComponent<TMP_Text>().text;
    private int CurrentHashedText => MarkdownText.GetHashCode();
    private MarkupSymbol LastSymbol => markupSymbols.TryLast(out MarkupSymbol symbol) ? symbol : MarkupSymbol.Empty;
    private bool HasLoggerAttached => gameObject.GetComponent<ConvertionLogger>() != null;
    private ConvertionLogger Logger => gameObject.GetComponent<ConvertionLogger>();
    public string CodeFontName => codeFont != null ? codeFont.name : "Consolas SDF";
    public string CodeFontColorHex => '#' + ColorUtility.ToHtmlStringRGBA(codeBackgroundColor);


    private int lastHash = 0;
    private readonly List<MarkupSymbol> markupSymbols = new();
    private readonly List<string> convertedText = new();
    private string currentBracketText = string.Empty, currentParenthesisText = string.Empty;

    [SerializeField] private bool autoConvertOnChange = true;
    [SerializeField] private TMP_FontAsset codeFont;
    [SerializeField] private Color codeBackgroundColor;


    void OnEnable()
    {
        lastHash = CurrentHashedText;
    }

    void Update()
    {
        if (!autoConvertOnChange) return;

        if (lastHash != CurrentHashedText)
        {
            if (HasLoggerAttached) Logger.Log(ConvertionLogger.LogType.Info, "TMP_Text updated");
            convertedText.Clear();
            markupSymbols.Clear();
            ConvertToTMPCompatibleText();
            lastHash = CurrentHashedText;
        }
    }

    public void ConvertToTMPCompatibleText()
    {
        // StringBuilder sb = new();
        int index = 0;
        foreach (char letter in MarkdownText)
        {
            if (LastSymbol.Symbol == "\\")
            {
                markupSymbols.Remove(LastSymbol);
                continue;
            }

            if (LastSymbol.Symbol.Contains("`") && letter != '`' && letter != '\n')
            {
                ReplaceLatestSymbol();
                convertedText.AppendToLatestStringInList(letter.ToString());
                continue;
            }

            if ((LastSymbol.Symbol == "![" || LastSymbol.Symbol == "[") && letter != ']')
            {
                currentBracketText += letter;
                continue;
            }

            if ((LastSymbol.Symbol == "![](" || LastSymbol.Symbol == "[](") && letter != ')')
            {
                currentParenthesisText += letter;
                continue;
            }

            if ((LastSymbol.Symbol == "![]") && letter != '(')
                ReplaceLatestSymbol(); //An image can exist without fallback text

            switch (letter)
            {
                case '\n':
                    LineEnded();
                    markupSymbols.AddOrJoin(new MarkupSymbol("\n"));
                    convertedText.Add("");
                    break;
                case '#':
                    if (LastSymbol.IsNewLine || LastSymbol.Symbol.Contains("#"))
                    {
                        if (markupSymbols.AddOrJoin(new MarkupSymbol("#", LastSymbol.IsNewLine), 6))
                            ReplaceLatestSymbol();
                    }
                    else convertedText.AppendToLatestStringInList("#");
                    break;
                case '>':
                    if (LastSymbol.IsNewLine || LastSymbol.Symbol.Contains(">"))
                    {
                        if (markupSymbols.AddOrJoin(new MarkupSymbol(">", LastSymbol.IsNewLine), 2))
                            ReplaceLatestSymbol();
                    }
                    else convertedText.AppendToLatestStringInList(">");
                    break;
                case '-':
                    // --- is a horizontal rule
                    // a line of only - is h2 for the line above it
                    if (LastSymbol.IsNewLine || LastSymbol.Symbol.Contains("-")) markupSymbols.AddOrJoin(new MarkupSymbol("-", true), int.MaxValue);
                    else convertedText.AppendToLatestStringInList("-");
                    break;
                case '`':
                    if (markupSymbols.AddOrJoin(new MarkupSymbol("`"), 3))
                        ReplaceLatestSymbol();
                    break;
                case ' ':
                    LastSymbol.HasSpaceAfter = true; //Mark that the last symbol had a space after it
                    if (LastSymbol.Symbol != "\n")
                        ReplaceLatestSymbol();
                    convertedText.AppendToLatestStringInList(" ");
                    break;
                case '_':
                case '*':
                    if (markupSymbols.AddOrJoin(new MarkupSymbol($"*", LastSymbol.IsNewLine), 3))
                        ReplaceLatestSymbol();
                    break;
                case '=':
                    if (LastSymbol.IsNewLine || LastSymbol.Symbol.Contains("="))
                        markupSymbols.AddOrJoin(new MarkupSymbol("=", LastSymbol.IsNewLine), int.MaxValue); //No limit
                    else convertedText.AppendToLatestStringInList("=");
                    break;
                case '!':
                    markupSymbols.AddOrJoin(new MarkupSymbol("!"), 1);
                    break;
                case '[':
                    if (LastSymbol.Symbol == "!") LastSymbol.Symbol += "[";
                    else markupSymbols.AddOrJoin(new MarkupSymbol("["));
                    break;
                case ']':
                    if (LastSymbol.Symbol == "![" || LastSymbol.Symbol == "[")
                        LastSymbol.Symbol += "]";
                    else convertedText.AppendToLatestStringInList("]");
                    break;
                case '(':
                    if (LastSymbol.Symbol == "![]" || LastSymbol.Symbol == "[]") LastSymbol.Symbol += "(";
                    else convertedText.AppendToLatestStringInList("(");
                    break;
                case ')':
                    if (LastSymbol.Symbol == "![](" || LastSymbol.Symbol == "[](") LastSymbol.Symbol += ")";
                    else convertedText.AppendToLatestStringInList(")");
                    break;
                case '~':
                    if (markupSymbols.AddOrJoin(new MarkupSymbol("~"), 2))
                        ReplaceLatestSymbol();
                    break;
                case '^':
                    markupSymbols.AddOrJoin(new MarkupSymbol("^"));
                    ReplaceLatestSymbol();
                    break;
                case '\\':
                    markupSymbols.AddOrJoin(new MarkupSymbol("\\"));
                    break;
                default:
                    ReplaceLatestSymbol();
                    convertedText.AppendToLatestStringInList(letter);
                    break;
            }
            index++;
        }

        LineEnded(true); //End the last line

        StringBuilder sb = new();
        foreach (string str in convertedText)
        {
            sb.Append(str);
            if (convertedText.Last() != str) sb.Append("\n"); //Add a new line at the end
        }
        gameObject.GetComponent<TMP_Text>().text = sb.ToString();
    }
    private void LineEnded(bool isEndOfText = false)
    {
        if (HasLoggerAttached) Logger.Log(ConvertionLogger.LogType.Info, "Line ended");
        markupSymbols.RemoveAll(s => s.Symbol == "\n"); //Remove previous line end
        if (markupSymbols.Any(s => s.Symbol.Contains("#"))) //end of header line
        {
            convertedText.AppendToLatestStringInList("</size>");
            int index = markupSymbols.FindLastIndex(s => s.Symbol.Contains("#"));
            if (index != -1)
                markupSymbols.RemoveAt(index);
        }

        if (markupSymbols.Any(s => s.Symbol == "`")) //Close single tick code
        {
            convertedText.AppendToLatestStringInList("</font></mark>");
            int index = markupSymbols.FindLastIndex(s => s.Symbol == "`");
            if (index != -1)
                markupSymbols.RemoveAt(index);
        }

        if (LastSymbol.Symbol.Contains("=") && convertedText.Last() == "")
        {
            convertedText.Remove(convertedText.Last());
            convertedText.AppendToLatestStringInList("</h2>"); //Add a horizontal rule
            convertedText.PreendToLatestStringInList("<h2>");
            markupSymbols.Remove(LastSymbol);
        }

        if (LastSymbol.Symbol.Contains("-") && convertedText.Last() == "" && convertedText.Count >= 2 && convertedText[^2] != "")
        {
            convertedText.Remove(convertedText.Last());
            convertedText.AppendToLatestStringInList("</h1>"); //Add a horizontal rule
            convertedText.PreendToLatestStringInList("<h1>");
            markupSymbols.Remove(LastSymbol);
        }

        if (LastSymbol.Symbol == "![" || LastSymbol.Symbol == "[") //Image/Link bracket opener wasnt closed treat as literal
        {
            convertedText.AppendToLatestStringInList($"{LastSymbol.Symbol}{currentBracketText}");
            currentBracketText = string.Empty;
            markupSymbols.Remove(LastSymbol);
        }

        if (LastSymbol.Symbol == "![](" || LastSymbol.Symbol == "[](") //Image/Link paranthesis opener wasnt closed treat as literal
        {
            convertedText.AppendToLatestStringInList($"{(LastSymbol.Symbol.First() == '!' ? '!' : "")}[{currentBracketText}]({currentParenthesisText}");
            currentBracketText = string.Empty;
            currentParenthesisText = string.Empty;
            markupSymbols.Remove(LastSymbol);
        }

        if (LastSymbol.Symbol.Contains("-") && convertedText.Last() == "" && convertedText.Count >= 2 && convertedText[^2] == "")
        {
            Debug.LogWarning("TMP does not support horizontal rules, please avoid using them in your text.");
            markupSymbols.Remove(LastSymbol);
        }

        if (isEndOfText)
        {
            if (markupSymbols.Any(s => s.Symbol == "*" && s.IsReplaced)) //Close italics
            {
                convertedText.AppendToLatestStringInList("</i>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "*");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }

            if (markupSymbols.Any(s => s.Symbol == "**" && s.IsReplaced)) //Close bold
            {
                convertedText.AppendToLatestStringInList("</b>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "**");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }

            if (markupSymbols.Any(s => s.Symbol == "***" && s.IsReplaced)) //Close both
            {
                convertedText.AppendToLatestStringInList("</b></i>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "***");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }


            if (markupSymbols.Any(s => s.Symbol == "```")) //Close triple tick code
            {
                convertedText.AppendToLatestStringInList("</font></mark>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "```");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }

            if (markupSymbols.Any(s => s.Symbol == "~~")) //Close strikethrough
            {
                convertedText.AppendToLatestStringInList("</s>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "~~");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }

            if (markupSymbols.Any(s => s.Symbol == "~")) //Close subscript
            {
                convertedText.AppendToLatestStringInList("</sub>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "~");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }

            if (markupSymbols.Any(s => s.Symbol == "^")) //Close superscript
            {
                convertedText.AppendToLatestStringInList("</sup>");
                int index = markupSymbols.FindLastIndex(s => s.Symbol == "^");
                if (index != -1)
                    markupSymbols.RemoveAt(index);
            }
        }

        ReplaceLatestSymbol();
    }

    private void ReplaceLatestSymbol()
    {
        MarkupSymbol mds = markupSymbols.TryLast(out MarkupSymbol top) ? top : MarkupSymbol.Empty;
        if (mds.IsEmpty || mds.IsReplaced) return;

        if (HasLoggerAttached) Logger.Log(ConvertionLogger.LogType.Replacement, $"Replacing symbol: {mds.Symbol}, IsOpener: {mds.IsOpener}");
        switch (mds.Symbol)
        {
            case "*":
                if (mds.IsAtLineStart && mds.HasSpaceAfter)
                {
                    convertedText.AppendToLatestStringInList("•");
                    markupSymbols.Remove(mds);
                    break;
                }

                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<i>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</i>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol.Equals("*"));
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "**":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<b>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</b>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol.Equals("**"));
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "***":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<i><b>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</b></i>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol.Equals("***"));
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "\n":
                markupSymbols.Remove(mds);
                break;
            case "#":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)

                {
                    convertedText.AppendToLatestStringInList("<size=160%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("#");
                markupSymbols.Remove(mds);
                break;
            case "##":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)
                {
                    convertedText.AppendToLatestStringInList("<size=150%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("##");
                markupSymbols.Remove(mds);
                break;
            case "###":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)
                {
                    convertedText.AppendToLatestStringInList("<size=140%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("###");
                markupSymbols.Remove(mds);
                break;
            case "####":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)
                {
                    convertedText.AppendToLatestStringInList("<size=130%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("####");
                markupSymbols.Remove(mds);
                break;
            case "#####":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)
                {
                    convertedText.AppendToLatestStringInList("<size=120%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("#####");
                markupSymbols.Remove(mds);
                break;
            case "######":
                if (mds.IsAtLineStart && mds.HasSpaceAfter && !mds.IsReplaced)
                {
                    convertedText.AppendToLatestStringInList("<size=110%>");
                    mds.IsReplaced = true;
                    break;
                }
                convertedText.AppendToLatestStringInList("######");
                markupSymbols.Remove(mds);
                break;
            case ">":
                if (mds.IsAtLineStart && mds.HasSpaceAfter)
                {
                    convertedText.AppendToLatestStringInList(" |");
                    markupSymbols.Remove(mds);
                    break;
                }
                convertedText.AppendToLatestStringInList(">");
                markupSymbols.Remove(mds);
                break;
            case ">>":
                if (mds.IsAtLineStart && mds.HasSpaceAfter)
                {
                    convertedText.AppendToLatestStringInList(" | |");
                    markupSymbols.Remove(mds);
                    break;
                }
                convertedText.AppendToLatestStringInList(">>");
                markupSymbols.Remove(mds);
                break;
            case "-":
                if (mds.IsAtLineStart && mds.HasSpaceAfter)
                {
                    convertedText.AppendToLatestStringInList("•");
                    markupSymbols.Remove(mds);
                    break;
                }
                convertedText.AppendToLatestStringInList("-");
                markupSymbols.Remove(mds);
                break;
            case "```":
            case "`":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList($"<mark={CodeFontColorHex}><font=\"{CodeFontName}\">");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</font></mark>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol == "`");
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "!":
                convertedText.AppendToLatestStringInList("!");
                markupSymbols.Remove(mds);
                break;
            case "![]":
            case "![]()":
                TMP_SpriteAsset sprite = Resources.Load<TMP_SpriteAsset>($"Sprite Assets/{currentBracketText}");
                if (sprite == null && currentParenthesisText != string.Empty) convertedText.AppendToLatestStringInList($"{currentParenthesisText}");
                else convertedText.AppendToLatestStringInList($"<sprite=\"{currentBracketText}\" index=0>");
                currentBracketText = string.Empty;
                currentParenthesisText = string.Empty;
                markupSymbols.Remove(mds);
                break;
            case "[]()":
                convertedText.AppendToLatestStringInList($"<link=\"{currentParenthesisText}\"><color=#1E90FF><u>{currentBracketText}</u></color></link>");
                currentBracketText = string.Empty;
                currentParenthesisText = string.Empty;
                markupSymbols.Remove(mds);
                break;
            case "~":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<sub>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</sub>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol == "~");
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "~~":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<s>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</s>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol == "~~");
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;
            case "^":
                if (mds.IsOpener)
                {
                    convertedText.AppendToLatestStringInList("<sup>");
                    mds.IsReplaced = true;
                }
                else
                {
                    convertedText.AppendToLatestStringInList("</sup>");
                    markupSymbols.Remove(mds);
                    int index = markupSymbols.FindLastIndex(s => s.Symbol == "^");
                    if (index != -1)
                        markupSymbols.RemoveAt(index);
                }
                break;

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(gameObject.GetComponent<TMP_Text>(), eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            var linkInfo = gameObject.GetComponent<TMP_Text>().textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();
            Application.OpenURL(url);
        }
    }
}
