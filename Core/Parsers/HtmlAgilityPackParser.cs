using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RazorMail.Extensions;

namespace RazorMail.Parsers
{
    public class HtmlAgilityPackParser : IParser
    {

        private Uri Uri { get; set; }
        protected IList<Action<HtmlNode>> Parsers { get; set; }

        public HtmlAgilityPackParser(Uri uri)
        {
            Uri = uri;
            Parsers = new List<Action<HtmlNode>>();

            Parsers.Add(ReplaceHrefElements);
            Parsers.Add(RemoveUnwantedElements);
            Parsers.Add(RemoveWhitespace);
            Parsers.Add(AppendBrToBlockElements);
            Parsers.Add(AppendTableDividers);
            Parsers.Add(ReplaceBrElements);
        }

        public virtual string BaseUrls(string html)
        {
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(html);

            Action<HtmlAttribute> @base = x => x.Value = x.Value.Contains("://") ? x.Value : Uri.GetLeftPart(UriPartial.Authority) + "/" + x.Value.TrimStart('/');
            doc.DocumentNode.SelectNodes("//a[@href]").ForEach(x => @base(x.Attributes["href"]));
            doc.DocumentNode.SelectNodes("//img[@src]").ForEach(x => @base(x.Attributes["src"]));

            return doc.DocumentNode.InnerHtml;
        }

        public virtual string StripHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(html);

            Parsers.ForEach(x => x(doc.DocumentNode));

            return Regex.Replace(doc.DocumentNode.InnerText, "[\r\n]{3,}", "\r\n\r\n");
        }

        protected virtual void ReplaceHrefElements(HtmlNode root)
        {
            root.SelectNodes("//a[@href]").ForEach(x =>
            {
                var href = x.Attributes["href"].Value.Replace("mailto:", string.Empty);
                var text = x.InnerText;

                if (string.IsNullOrWhiteSpace(text))
                    text = x.FirstChild.Attributes["alt"] != null ? x.FirstChild.Attributes["alt"].Value : href;

                HtmlNode link = HtmlTextNode.CreateNode(text + (href != text ? " [ " + href + " ]" : ""));

                x.ParentNode.ReplaceChild(link, x);
            });
        }

        protected virtual void RemoveUnwantedElements(HtmlNode root)
        {
            root.SelectNodes("//style|//img|//comment()").ForEach(x => x.Remove());
        }

        protected virtual void RemoveWhitespace(HtmlNode root)
        {
            root.DescendantNodes().ForEach(x => x.InnerHtml = Regex.Replace(x.InnerHtml, "\\s+", " "));
            root.DescendantNodes().ForEach(x => x.InnerHtml = x.InnerHtml.Trim());
        }

        protected virtual void AppendBrToBlockElements(HtmlNode root)
        {
            var blocks = new[] { "//address", "//blockquote", "//center", "//div", "//dl", "//fieldset", "//h1", "//h2", "//h3", "//h4", "//h5", "//h6", "//hr", "//li", "//p", "//pre" };
            root.SelectNodes(string.Join("|", blocks)).ForEach(x => x.AppendChild(HtmlTextNode.CreateNode("<br />")));
        }

        protected virtual void AppendTableDividers(HtmlNode root)
        {
            root.SelectNodes("//td[position()<last()]|//th[position()<last()]").Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ForEach(x => x.AppendChild(HtmlTextNode.CreateNode(", ")));
            root.SelectNodes("//td[last()]|//th[last()]").ForEach(x => x.AppendChild(HtmlTextNode.CreateNode("<br />")));
        }

        protected virtual void ReplaceBrElements(HtmlNode root)
        {
            root.SelectNodes("//br").ForEach(x => x.ParentNode.ReplaceChild(HtmlTextNode.CreateNode("\r\n"), x));
        }

    }


    
}
