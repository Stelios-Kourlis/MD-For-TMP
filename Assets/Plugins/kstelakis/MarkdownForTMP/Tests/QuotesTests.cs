using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class QuotesTests
{
    private static TMP_Text tmp;
    private static GameObject go;

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
    public IEnumerator TestQuote()
    {
        tmp.text = "> Quote";
        yield return null;
        Debug.Log(tmp.text);
        Assert.AreEqual(" | Quote", tmp.text, "> should be converted to a quote");
    }

    [UnityTest]
    public IEnumerator TestDoubleQuote()
    {
        tmp.text = ">> Quote";
        yield return null;
        Assert.AreEqual(" | | Quote", tmp.text, ">> should be converted to a double quote");
    }

    [UnityTest]
    public IEnumerator TestFalseQuote()
    {
        tmp.text = ">Quote";
        yield return null;
        Assert.AreEqual(">Quote", tmp.text, "> with not space shouldn't be converted to a quote");
    }

    [UnityTest]
    public IEnumerator TestFalseDoubleQuote()
    {
        tmp.text = ">>Quote";
        yield return null;
        Assert.AreEqual(">>Quote", tmp.text, ">>  with no space shouldn't be converted to a double quote");
    }

    [UnityTest]
    public IEnumerator TestMiddleQuote()
    {
        tmp.text = "Not> Quote";
        yield return null;
        Assert.AreEqual("Not> Quote", tmp.text, "> in the middle of a sentence shouldn't be converted to a quote");
    }
}
