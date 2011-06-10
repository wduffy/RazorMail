using System.IO;

namespace RazorMail
{
    internal class FileReader : IFileReader
    {

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

    }
}
