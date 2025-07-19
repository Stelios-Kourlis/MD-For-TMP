using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class LinksTests
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
    public IEnumerator TestLink()
    {
        tmp.text = "[test](https://example.com)";
        yield return null;
        Assert.AreEqual("<link=\"https://example.com\"><color=#1E90FF><u>test</u></color></link>", tmp.text, "Link should be converted to a link tag with the correct URL");
    }

    [UnityTest]
    public IEnumerator TestLinkWithMissingURL()
    {
        tmp.text = "[test]()";
        yield return null;
        Assert.AreEqual("<link=\"\"><color=#1E90FF><u>test</u></color></link>", tmp.text, "Link with missing URL should still be converted to a link tag with an empty URL");
    }

    [UnityTest]
    public IEnumerator TestUnclosedLink()
    {
        tmp.text = "[test](https://example.com";
        yield return null;
        Assert.AreEqual("[test](https://example.com", tmp.text, "Unclosed link should be treated as literal text");
    }

    [UnityTest]
    public IEnumerator TestLinkWithoutTitle()
    {
        tmp.text = "[](https://example.com)";
        yield return null;
        Assert.AreEqual("<link=\"https://example.com\"><color=#1E90FF><u></u></color></link>", tmp.text, "Link with no title should still be converted to a link tag with an empty title");
    }
}
