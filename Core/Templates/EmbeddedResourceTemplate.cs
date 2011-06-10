using System.IO;
using System.Reflection;

namespace RazorMail.Templates
{

    public class EmbeddedResourceTemplate : ITemplate
    {
        private Assembly _assembly;
        private string _resource;

        public EmbeddedResourceTemplate(Assembly assembly, string resource)
        {
            _assembly = assembly;
            _resource = string.Format("{0}.{1}", assembly.GetName().Name, resource);
        }

        public void Write(Stream template)
        {
            using (var stream = _assembly.GetManifestResourceStream(_resource))
                stream.CopyTo(template);
        }
    }

}