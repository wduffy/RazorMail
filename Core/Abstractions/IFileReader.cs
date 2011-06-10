using System.IO;

namespace RazorMail
{
    public interface IFileReader
    {
        bool Exists(string path);
        Stream OpenRead(string path);
    }
}
