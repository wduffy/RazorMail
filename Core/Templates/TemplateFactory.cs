using System.Linq;
using System.Reflection;

namespace RazorMail.Templates
{

    public class TemplateFactory  : ITemplateFactory
    {

        private IFileReader FileReader { get; set; }
        private Assembly Assembly { get; set; }

        public TemplateFactory(IFileReader fileReader, Assembly assembly)
        {
            FileReader = fileReader;
            Assembly = assembly;
        }

        public ITemplate Create(string template)
        {
            // Is this template an embedded resource?
            if (Assembly.GetManifestResourceNames().Contains(string.Format("{0}.{1}", Assembly.GetName().Name, template)))
                return new EmbeddedResourceTemplate(Assembly, template);

            // Is this template a file resource?
            if (FileReader.Exists(template))
                return new FileTemplate(FileReader, template);

            // It must be a basic string template
            return new StringTemplate(template);
        }

    }

}