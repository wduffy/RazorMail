using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorMail.Parsers
{
    public interface IParser
    {
        string BaseUrls(string html);
        string StripHtml(string html);
    }
}
