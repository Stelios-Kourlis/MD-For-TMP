using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class ImagesTests
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
}
