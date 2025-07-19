using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace kstelakis.MarkdownForTMP
{

    public class MiscTests
    {
        private static TMP_Text tmp;
        private static GameObject go;

        [OneTimeSetUp] // Runs once before all tests
        public void Setup()
        {
            go = new();
            tmp = go.AddComponent<TextMeshProUGUI>();
            go.AddComponent<MarkdownFormatter>();
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

        [UnityTest]
        public IEnumerator TestEscapeSeq()
        {
            tmp.text = "Literal \\*";
            yield return null;
            Assert.AreEqual("Literal *", tmp.text, "\\ should escape the next character");
        }

        [UnityTest]
        public IEnumerator TestEscapeSeqOnSelf()
        {
            tmp.text = "Literal \\\\";
            yield return null;
            Assert.AreEqual("Literal \\", tmp.text, "\\ should escape even itself");
        }

        [UnityTest]
        public IEnumerator TestParenthesisOpening()
        {
            tmp.text = "(secret)";
            yield return null;
            Assert.AreEqual("(secret)", tmp.text, "Parenthesis should not be converted to a link/image when not preceded by []");
        }
    }
}
