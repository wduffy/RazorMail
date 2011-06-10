using System.IO;

namespace RazorMail.Templates
{

    public interface ITemplate
    {
        void Write(Stream template);
    }

}