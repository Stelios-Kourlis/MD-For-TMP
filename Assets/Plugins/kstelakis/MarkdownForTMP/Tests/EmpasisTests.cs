using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class EmpasisTests
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
    public IEnumerator TestItalics()
    {
        tmp.text = "*Italic Text*";
        yield return null;
        Assert.AreEqual("<i>Italic Text</i>", tmp.text, "Italic text should be converted to &lt;i&gt; tags");
    }

    [UnityTest]
    public IEnumerator TestBold()
    {
        tmp.text = "**Bold Text**";
        yield return null;
        Assert.AreEqual("<b>Bold Text</b>", tmp.text, "Bold text should be converted to &lt;b&gt; tags");
    }

    [UnityTest]
    public IEnumerator TestBoldAndItalics()
    {
        tmp.text = "***Bold and Italic Text***";
        yield return null;
        Assert.AreEqual("<i><b>Bold and Italic Text</b></i>", tmp.text, "Bold and italic text should be converted to &lt;b&gt; and &lt;i&gt; tags");
    }

    [UnityTest]
    public IEnumerator TestMixedBoldAndItalics()
    {
        tmp.text = "*I**B**I*";
        yield return null;
        Assert.AreEqual("<i>I<b>B</b>I</i>", tmp.text, "Nested bold and italic text should be converted correctly");

        tmp.text = "**B*I*B**";
        yield return null;
        Assert.AreEqual("<b>B<i>I</i>B</b>", tmp.text, "Nested bold and italic text should be converted correctly");
    }

    [UnityTest]
    public IEnumerator TestUnclosedItalics()
    {
        tmp.text = "*Unclosed Italic Text";
        yield return null;
        Assert.AreEqual("<i>Unclosed Italic Text</i>", tmp.text, "Unclosed italic text should be closed automatically");
    }

    [UnityTest]
    public IEnumerator TestUnclosedBold()
    {
        tmp.text = "Unclosed **Bold Text";
        yield return null;
        Assert.AreEqual("Unclosed <b>Bold Text</b>", tmp.text, "Unclosed bold text should be closed automatically");
    }

    [UnityTest]
    public IEnumerator TestUnclosedBoth()
    {
        tmp.text = "Unclosed ***Bold and italic Text";
        yield return null;
        Assert.AreEqual("Unclosed <i><b>Bold and italic Text</b></i>", tmp.text, "Unclosed bold and italic text should be closed automatically");
    }

    [UnityTest]
    public IEnumerator TestUnclosedMixed()
    {
        tmp.text = "*Unclosed **Bold and Italic Text";
        yield return null;
        Assert.AreEqual("<i>Unclosed <b>Bold and Italic Text</i></b>", tmp.text, "Unclosed mixed text should be closed automatically");
    }
}
