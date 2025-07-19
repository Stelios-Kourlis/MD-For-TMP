using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace kstelakis.MarkdownForTMP
{
    public class ListsTests
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
        public IEnumerator TestUnorderedListDashes()
        {
            tmp.text = "- Item 1\n- Item 2\n- Item 3";
            yield return null;
            Assert.AreEqual("• Item 1\n• Item 2\n• Item 3", tmp.text, "Unordered dash list should be converted to bullet points");
        }

        [UnityTest]
        public IEnumerator TestUnorderedListAsterisks()
        {
            tmp.text = "* Item 1\n* Item 2\n* Item 3";
            yield return null;
            Assert.AreEqual("• Item 1\n• Item 2\n• Item 3", tmp.text, "Unordered asterisk list should be converted to bullet points");
        }
    }
}