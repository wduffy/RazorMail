using System;
using HtmlAgilityPack;
using RazorMail.Parsers;

namespace RazorMail.Tests
{
    public class HtmlAgilityPackParserTester : HtmlAgilityPackParser
    {

        public HtmlAgilityPackParserTester(Uri uri) : base(uri) { }

        public void TestAppendBrToBlockElements(HtmlNode root)
        {
            AppendBrToBlockElements(root);
        }

        public void TestAppendTableDividers(HtmlNode root)
        {
            AppendTableDividers(root);
        }

        public void TestRemoveUnwantedElements(HtmlNode root)
        {
            RemoveUnwantedElements(root);
        }

        public void TestRemoveWhitespace(HtmlNode root)
        {
            RemoveWhitespace(root);
        }

        public void TestReplaceBrElements(HtmlNode root)
        {
            ReplaceBrElements(root);
        }

        public void TestReplaceHrefElements(HtmlNode root)
        {
            ReplaceHrefElements(root);
        }

    }
}