using System;
using System.IO;

namespace RazorMail.Templates
{
    
    internal class StringTemplate : ITemplate
    {
        private string _template;

        public StringTemplate(string template)
        {
            _template = template;
        }

        public void Write(Stream template)
        {
            var writer = new StreamWriter(template);
            writer.Write(_template);
            writer.Flush();
        }
    }

}
