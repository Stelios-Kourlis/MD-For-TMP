using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class HeadersTests
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
    public IEnumerator TestHeading1()
    {
        tmp.text = "# Heading 1";
        yield return null;
        Assert.AreEqual("<size=160%> Heading 1</size>", tmp.text, "Heading 1 should be converted to size 160%");
    }

    [UnityTest]
    public IEnumerator TestHeading2()
    {
        tmp.text = "## Heading 2";
        yield return null;
        Assert.AreEqual("<size=150%> Heading 2</size>", tmp.text, "Heading 2 should be converted to size 150%");
    }

    [UnityTest]
    public IEnumerator TestHeading3()
    {
        tmp.text = "### Heading 3";
        yield return null;
        Assert.AreEqual("<size=140%> Heading 3</size>", tmp.text, "Heading 3 should be converted to size 140%");
    }

    [UnityTest]
    public IEnumerator TestHeading4()
    {
        tmp.text = "#### Heading 4";
        yield return null;
        Assert.AreEqual("<size=130%> Heading 4</size>", tmp.text, "Heading 4 should be converted to size 130%");
    }

    [UnityTest]
    public IEnumerator TestHeading5()
    {
        tmp.text = "##### Heading 5";
        yield return null;
        Assert.AreEqual("<size=120%> Heading 5</size>", tmp.text, "Heading 5 should be converted to size 120%");
    }

    [UnityTest]
    public IEnumerator TestHeading6()
    {
        tmp.text = "###### Heading 6";
        yield return null;
        Assert.AreEqual("<size=110%> Heading 6</size>", tmp.text, "Heading 6 should be converted to size 110%");
    }

    [UnityTest]
    public IEnumerator TestFalseHeading()
    {
        tmp.text = "##Not a heading";
        yield return null;
        Assert.AreEqual("##Not a heading", tmp.text, "# not followed aren't headings");
    }

    [UnityTest]
    public IEnumerator TestMiddleHeading()
    {
        tmp.text = "Not ## a heading";
        yield return null;
        Assert.AreEqual("Not ## a heading", tmp.text, "# in the middle of a sentence shouldn't be converted to a heading");
    }

    [UnityTest]
    public IEnumerator TestUnderlineHeading2()
    {
        tmp.text = "Heading 2\n==========";
        yield return null;
        Assert.AreEqual("<h2>Heading 2</h2>", tmp.text, "= under a line should convert the line to a heading 2");
    }

    [UnityTest]
    public IEnumerator TestUnderlineHeading1()
    {
        tmp.text = "Heading 1\n----------";
        yield return null;
        Assert.AreEqual("<h1>Heading 1</h1>", tmp.text, "- under a line should convert the line to a heading 1");
    }
}
