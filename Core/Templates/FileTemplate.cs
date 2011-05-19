using System;
using System.IO;

namespace RazorMail.Templates
{

    internal class FileTemplate : ITemplate
    {
        private string _path;

        public FileTemplate(string path)
        {
            _path = path;
        }

        public void Write(Stream template)
        {
            using (var stream = File.OpenRead(_path))
                stream.CopyTo(template);
        }
    }

}
