using System.IO;

namespace RazorMail.Templates
{

    public class FileTemplate : ITemplate
    {
        private IFileReader _reader;
        private string _path;

        public FileTemplate(IFileReader reader, string path)
        {
            _reader = reader;
            _path = path;
        }

        public void Write(Stream template)
        {
            using (var stream = _reader.OpenRead(_path))
                stream.CopyTo(template);
        }
    }

}
