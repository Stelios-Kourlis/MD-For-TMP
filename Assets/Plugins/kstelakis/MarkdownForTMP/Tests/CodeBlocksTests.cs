using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class CodeBlocksTests
{
    private static TMP_Text tmp;
    private static GameObject go;
    private static string CodeBlockFontName => go.GetComponent<MarkdownToTMPConverter>().CodeFontName;
    private static string CodeBlockColorHex => go.GetComponent<MarkdownToTMPConverter>().CodeFontColorHex;

    [OneTimeSetUp] // Runs once before all tests
    public void Setup()
    {
        go = new();
        tmp = go.AddComponent<TextMeshProUGUI>();
        go.AddComponent<MarkdownToTMPConverter>();
        go.AddComponent<ConvertionLogger>();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator TestSingleLineCodeBlock()
    {
        tmp.text = "`Single line code`";
        yield return null;
        Assert.AreEqual($"<mark={CodeBlockColorHex}><font=\"{CodeBlockFontName}\">Single line code</font></mark>", tmp.text, "Single line code should use code font tags");
    }

    [UnityTest]
    public IEnumerator TestSingleLineCodeBlockWithNestedMarkdown()
    {
        tmp.text = "`# Text * and ** more *** text`";
        yield return null;
        Assert.AreEqual($"<mark={CodeBlockColorHex}><font=\"{CodeBlockFontName}\"># Text * and ** more *** text</font></mark>", tmp.text, "Single line code should ignore nestd markdown");
    }

    [UnityTest]
    public IEnumerator TestUnclosedSingleLine()
    {
        tmp.text = "`Unclosed";
        yield return null;
        Assert.AreEqual($"<mark={CodeBlockColorHex}><font=\"{CodeBlockFontName}\">Unclosed</font></mark>", tmp.text, "Single line code should be automatically be closed at the end");
    }

    [UnityTest]
    public IEnumerator TestMultiLineCodeBlock()
    {
        tmp.text = "```\nMulti\nline\ncode```";
        yield return null;
        Assert.AreEqual($"<mark={CodeBlockColorHex}><font=\"{CodeBlockFontName}\">\nMulti\nline\ncode</font></mark>", tmp.text, "Multi-line code should use code font tags and preserve new lines");
    }

    [UnityTest]
    public IEnumerator TestUnclosedMultiLineCodeBlock()
    {
        tmp.text = "```\nMulti\nline\ncode";
        yield return null;
        Assert.AreEqual($"<mark={CodeBlockColorHex}><font=\"{CodeBlockFontName}\">\nMulti\nline\ncode</font></mark>", tmp.text, "Multi-line code should use code font tags and preserve new lines");
    }
}
