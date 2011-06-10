
namespace RazorMail.Templates
{

    public interface ITemplateFactory
    {
        ITemplate Create(string template);
    }

}