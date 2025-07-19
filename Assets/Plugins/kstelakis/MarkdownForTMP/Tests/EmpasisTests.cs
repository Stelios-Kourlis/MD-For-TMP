using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class EmpasisTests
{
    private static TMP_Text tmp;
    private static GameObject go;
    private static string HighlightedTextBackgroundColorHex => go.GetComponent<MarkdownToTMPConverter>().HighlightedTextBackgroundColorHex;

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

    [UnityTest]
    public IEnumerator TestStrikethrough()
    {
        tmp.text = "~~ Strikethrough Text ~~";
        yield return null;
        Assert.AreEqual("<s> Strikethrough Text </s>", tmp.text, "Strikethrough text should be converted to s tags");
    }

    [UnityTest]
    public IEnumerator TestUnclosedStrikethrough()
    {
        tmp.text = "~~ Strikethrough Text";
        yield return null;
        Assert.AreEqual("<s> Strikethrough Text</s>", tmp.text, "Unclosed Strikethrough text should be converted to s tags");
    }

    [UnityTest]
    public IEnumerator TestSuperscript()
    {
        tmp.text = "y = x^2^";
        yield return null;
        Assert.AreEqual("y = x<sup>2</sup>", tmp.text, "Superscript should be converted to sup tags");
    }

    [UnityTest]
    public IEnumerator TestUnclosedSuperscript()
    {
        tmp.text = "y = x^2";
        yield return null;
        Assert.AreEqual("y = x<sup>2</sup>", tmp.text, "Unclosed Superscript should be converted to sup tags");
    }

    [UnityTest]
    public IEnumerator TestSubscript()
    {
        tmp.text = "H~2~O";
        yield return null;
        Assert.AreEqual("H<sub>2</sub>O", tmp.text, "Subscript text should be converted to sub tags  and closed automatically");
    }

    [UnityTest]
    public IEnumerator TestUnclosedSubscript()
    {
        tmp.text = "H~2O";
        yield return null;
        Assert.AreEqual("H<sub>2O</sub>", tmp.text, "Unclosed Subscript text should be converted to sub tags  and closed automatically");
    }

    [UnityTest]
    public IEnumerator TestUnderlined()
    {
        tmp.text = "~~~dummy link~~~";
        yield return null;
        Assert.AreEqual("<u>dummy link</u>", tmp.text, "Triple tildes should be converted to underline tags");
    }

    [UnityTest]
    public IEnumerator TestUnclosedUnderlined()
    {
        tmp.text = "~~~dummy link";
        yield return null;
        Assert.AreEqual("<u>dummy link</u>", tmp.text, "Unclosed Triple tildes should be converted to underline tags and closed automatically");
    }

    [UnityTest]
    public IEnumerator TestHighlight()
    {
        tmp.text = "==fancy==";
        yield return null;
        Assert.AreEqual($"<mark={HighlightedTextBackgroundColorHex}>fancy</mark>", tmp.text, "== should be converted to highlight tags with the correct background color");
    }

    [UnityTest]
    public IEnumerator TestUnclosedHighlight()
    {
        tmp.text = "==fancy";
        yield return null;
        Assert.AreEqual("<mark={HighlightedTextBackgroundColorHex}>fancy</mark>", tmp.text, "Unclosed == should be converted to highlight tags with the correct background color and closed automatically");
    }
}
