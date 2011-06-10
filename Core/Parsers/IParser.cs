
namespace RazorMail.Parsers
{
    public interface IParser
    {
        string BaseUrls(string html);
        string StripHtml(string html);
    }
}
