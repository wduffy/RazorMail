using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RazorMail.Properties;

namespace RazorMail.Templates
{
    public class TemplateCollection : IEnumerable<ITemplate>
    {

        private IList<ITemplate> _templates;
        private IFileReader _fileReader;
        private Assembly _assembly;

        // Initializes an empty instance of the RazorMail.RazorMailAddressCollection class.
        public TemplateCollection(Assembly assembly, IFileReader fileReader)
        {
            if (assembly == null) throw new ArgumentException(Resources.ExceptionArgumentNullOrEmpty, "assembly");
            if (fileReader == null) throw new ArgumentException(Resources.ExceptionArgumentNullOrEmpty, "fileReader");

            _templates = new List<ITemplate>();
            _assembly = assembly;
            _fileReader = fileReader;
        }

        /// <summary>
        /// Adds a template to the collection.
        /// </summary>
        /// <param name="template">A System.String that contains the template to be added. Can be an assembly resource name, a file path or an html string.</param>
        public void Add(string template)
        {
            _templates.Add(CreateTemplate(template));
        }

        protected ITemplate CreateTemplate(string template)
        {
            // Is this template an embedded resource?
            if (_assembly.GetManifestResourceNames().Contains(string.Format("{0}.{1}", _assembly.GetName().Name, template)))
                return new EmbeddedResourceTemplate(_assembly, template);

            // Is this template a file resource?
            if (_fileReader.Exists(template))
                return new FileTemplate(_fileReader, template);

            // It must be a basic string template
            return new StringTemplate(template);
        }

        #region IEnumerable Members

        public IEnumerator<ITemplate> GetEnumerator()
        {
            return _templates.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _templates.GetEnumerator();
        }

        #endregion
    }
}
