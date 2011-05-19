using System;
using System.Reflection;
using System.IO;

namespace RazorMail.Templates
{

    internal class EmbeddedResourceTemplate : ITemplate
    {
        private string _resource;
        private Assembly _assembly;

        public EmbeddedResourceTemplate(string resource, Assembly assembly)
        {
            _resource = string.Format("{0}.{1}", assembly.GetName().Name, resource);
            _assembly = assembly;
        }

        public void Write(Stream template)
        {
            using (var stream = _assembly.GetManifestResourceStream(_resource))
                stream.CopyTo(template);
        }
    }

}