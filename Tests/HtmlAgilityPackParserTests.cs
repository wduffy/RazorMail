using System;
using HtmlAgilityPack;
using NUnit.Framework;
using RazorMail.Parsers;

namespace RazorMail.Tests
{
    [TestFixture]
    public class HtmlAgilityPackParserTests
    {

        protected Uri Uri { get; set; }
        protected HtmlDocument Html { get; set; }

        [SetUp]
        public void Setup()
        {
            Uri = new Uri("http://www.wduffy.co.uk");
            Html = new HtmlDocument();
            Html.OptionFixNestedTags = true;
        }

        [Test]
        public void BaseUrls_DoesBaseRelativeUrls()
        {
            // Arrange
            var parser = new HtmlAgilityPackParser(Uri);

            // Act
            var result = parser.BaseUrls("<a href=\"/test.html\"><img src=\"/image.jpg\" /></a>");

            //Assert
            Assert.That(result, Is.StringContaining(Uri.Host + "/test.html"));
            Assert.That(result, Is.StringContaining(Uri.Host + "/image.jpg"));
        }

        [Test]
        public void StripHtml_DoesRemoveAllHtml()
        {
            // Arrange
            var parser = new HtmlAgilityPackParser(Uri);

            // Act
            var result = parser.StripHtml("<html><head><style>.css { padding: 10px; }</style></head><body><h1>Heading</h1><div class=\"css\">This is a test.</div>With an image.<img src=\"image.jpg\" alt=\"image\" /></body></html>");

            //Assert
            Assert.That(result, Is.EqualTo("Heading\r\nThis is a test.\r\nWith an image."));
        }

        [Test]
        public void AppendBrToBlockElements_DoesAppendBrElementsToBlockElements()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body><h1>Heading</h1><div>This is a test</div>.</body></html>");

            // Act
            parser.TestAppendBrToBlockElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerHtml, Is.StringContaining("<br></h1>"));
            Assert.That(Html.DocumentNode.InnerHtml, Is.StringContaining("<br></div>"));
        }

        [Test]
        public void AppendTableDividers_DoesInsertTableCellDividers()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body><table><tr><th>Column 1</th><th>Column 2</th></tr><tr><td>Row 1-A</td><td>Row 1-B</td></tr><tr><td>Row 2-A</td><td>Row 2-B</td></tr></table></body></html>");

            // Act
            parser.TestAppendTableDividers(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("Column 1, Column 2"));
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("Row 1-A, Row 1-B"));
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("Row 2-A, Row 2-B"));
        }

        [Test]
        public void RemoveUnwantedElements_DoesRemoveUnwantedElements()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><head><style>.test { margin: 5px; }</style></head><body><img src=\"test.jpg\" alt=\"test\" /><!-- this is a test comment --></body></html>");

            // Act
            parser.TestRemoveUnwantedElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.Empty);
        }

        [Test]
        public void RemoveWhiteSpace_DoesRemoveExcesswhiteSpace()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This     is a      test  with       whitespace   \r\n   \r\n\r\n    to       remove.</body></html>");

            // Act
            parser.TestRemoveWhitespace(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.EqualTo("This is a test with whitespace to remove."));
        }

        [Test]
        public void ReplaceBrElements_DoesReplaceBrElementsWithNewLines()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This<br />is<br /><br /><br />a<br />test.</body></html>");

            // Act
            parser.TestReplaceBrElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerHtml, Is.Not.StringContaining("<br />"));
        }

        [Test]
        public void ReplaceHrefElements_WhenLinkIsEmailAddress_DoesReplaceHrefWithEmailAddressWithoutMailTo()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This is a test <a href=\"mailto:" + ObjectMother.TestAddress +"\">email address</a>.</body></html>");

            // Act
            parser.TestReplaceHrefElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("This is a test email address [ " + ObjectMother.TestAddress + " ]."));
        }

        [Test]
        public void ReplaceHrefElements_WhenLinkTextIsAvailable_DoesReplaceHrefElementsWithTextAndUrl()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This is a test <a href=\"http://www.wduffy.co.uk\">link</a>.</body></html>");

            // Act
            parser.TestReplaceHrefElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("This is a test link [ http://www.wduffy.co.uk ]."));
        }

        [Test]
        public void ReplaceHrefElements_WhenLinkIsImageWithAltTag_DoesReplaceHrefElementsWithAltTagAndUrl()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This is a test <a href=\"http://www.wduffy.co.uk\"><img src=\"test.jpg\" alt=\"image\" /></a> link.</body></html>");

            // Act
            parser.TestReplaceHrefElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("This is a test image [ http://www.wduffy.co.uk ] link."));
        }

        [Test]
        public void ReplaceHrefElements_WhenLinkIsImageWithNoAltTag_DoesReplaceHrefElementsWithUrl()
        {
            // Arrange
            var parser = new HtmlAgilityPackParserTester(Uri);
            Html.LoadHtml("<html><body>This is a test <a href=\"http://www.wduffy.co.uk\"><img src=\"test.jpg\" /></a> image link.</body></html>");

            // Act
            parser.TestReplaceHrefElements(Html.DocumentNode);

            // Assert
            Assert.That(Html.DocumentNode.InnerText, Is.StringContaining("This is a test http://www.wduffy.co.uk image link."));
        }

    }
}
