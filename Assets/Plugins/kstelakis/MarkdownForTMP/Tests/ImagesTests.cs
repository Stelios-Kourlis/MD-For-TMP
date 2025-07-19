using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class ImagesAndLinksTests
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
    public IEnumerator TestImage()
    {
        tmp.text = "![TMP Markdown Support Icon]()";
        yield return null;
        Assert.AreEqual($"<sprite=\"TMP Markdown Support Icon\" index=0>", tmp.text, "Image should be converted to sprite tag with index 0");
    }

    [UnityTest]
    public IEnumerator TestUnclosedImage()
    {
        tmp.text = "![TMP Markdown Support Icon";
        yield return null;
        Assert.AreEqual($"![TMP Markdown Support Icon", tmp.text, "Unclosed image should be treated as literal text");
    }

    [UnityTest]
    public IEnumerator TestImageWithoutFallbackText()
    {
        tmp.text = "![TMP Markdown Support Icon]";
        yield return null;
        Assert.AreEqual($"<sprite=\"TMP Markdown Support Icon\" index=0>", tmp.text, "Images without fallback text should still be converted to sprite tag with index 0");
    }

    [UnityTest]
    public IEnumerator TestNonExistingImageWithFallbackText()
    {
        tmp.text = "![B](oops!)";
        yield return null;
        Assert.AreEqual($"oops!", tmp.text, "Non-existing image with fallback text should be converted to the fallback text");
    }

    [UnityTest]
    public IEnumerator TestNonExistingImageWithoutFallbackText()
    {
        tmp.text = "![B]";
        yield return null;
        Assert.AreEqual($"<sprite=\"B\" index=0>", tmp.text, "Non-existing image with fallback text should be converted to the fallback text");
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
