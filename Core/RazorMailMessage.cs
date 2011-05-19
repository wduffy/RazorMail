using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using RazorEngine;
using RazorMail.Templates;
using System.Net.Mime;

namespace RazorMail
{

    /// <summary>
    /// Represents a razor e-mail message that can be sent using a concrete implementaion of the RazorMail.IRazorMailSender interface.
    /// </summary>
    public class RazorMailMessage
    {
        public string Subject { get; set; }
        public MailPriority Priority { get; set; }
        public Encoding Encoding { get; set; }
        public string PlainText { get; set; }
        internal MailAddress From { get; set; }
        internal MailAddressCollection To { get; set; }
        internal MailAddressCollection Cc { get; set; }
        internal MailAddressCollection Bcc { get; set; }
        internal MailAddressCollection ReplyTo { get; set; }
        internal IList<Attachment> Attachments { get; set; }
        public dynamic Set { get; private set; }
        private List<ITemplate> Templates { get; set; }

        /// <summary>
        /// Initialises an empty instance of the RazorMail.RazorMailmessage class.
        /// </summary>
        /// <param name="subject">A System.String that contains the subject text.</param>
        public RazorMailMessage(string subject)
        {
            Subject = subject;
            To = new MailAddressCollection();
            Cc = new MailAddressCollection();
            Bcc = new MailAddressCollection();
            ReplyTo = new MailAddressCollection();
            Attachments = new List<Attachment>();
            
            Set = new ExpandoObject();
            Templates = new List<ITemplate>();
        }

        #region Mail Addresses

        /// <summary>
        /// Adds a recipient to this message
        /// </summary>
        /// <param name="address">The address of this recipient</param>
        /// <param name="displayName">The name of this recipient</param>
        public void AddTo(string address, string displayName)
        {
            To.Add(new MailAddress(address, displayName));
        }

        /// <summary>
        /// Adds a reply-to recipient to this message
        /// </summary>
        /// <param name="address">The address of the reply-to recipient</param>
        /// <param name="displayName">The name of the reply-to recipient</param>
        public void AddReplyTo(string address, string displayName)
        {
            ReplyTo.Add(new MailAddress(address, displayName));
        }

        /// <summary>
        /// Adds a carbon copy recipient to this message
        /// </summary>
        /// <param name="address">The address of the carbon copy recipient</param>
        /// <param name="displayName">The name of the carbon copy recipient</param>
        public void AddCc(string address, string displayName)
        {
            Cc.Add(new MailAddress(address, displayName));
        }

        /// <summary>
        /// Adds a blind carbon copy recipient to this message
        /// </summary>
        /// <param name="address">The address of the blind carbon copy recipient</param>
        /// <param name="displayName">The name of the blind carbon copy recipient</param>
        public void AddBcc(string address, string displayName)
        {
            Bcc.Add(new MailAddress(address, displayName));
        }

        /// <summary>
        /// Sets the 'from' identity of this message
        /// </summary>
        /// <param name="address">The address of the sender</param>
        /// <param name="displayName">The name of the sender</param>
        public void AddFrom(string address, string displayName)
        {
            From = new MailAddress(address, displayName);
        }

        #endregion
        #region Attachments

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="fileName">A System.String that contains a file path to use to create this attachment.</param>
        public void AddAttachment(string fileName)
        {
            Attachments.Add(new Attachment(fileName));
        }

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="fileName">A System.String that contains a file path to use to create this attachment.</param>
        /// <param name="contentType">A System.Net.Mime.ContentType that describes the data in sting.</param>
        public void AddAttachment(string fileName, ContentType contentType)
        {
            Attachments.Add(new Attachment(fileName, contentType));
        }

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="fileName">A System.String that contains a file path to use to create this attachment.</param>
        /// <param name="mediaType">A System.String that contains the MIME Content-Header information for this attachment. This value can be null.</param>
        public void AddAttachment(string fileName, string mediaType)
        {
            Attachments.Add(new Attachment(fileName, mediaType));
        }

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="contentStream">A readable System.IO.Stream that contains the content for this attachment.</param>
        /// <param name="name">A System.String that contains the value for the System.Net.Mime.ContentType.Name property of the System.Net.Mime.ContentType associated with this attachment. This value can be null.</param>
        public void AddAttachment(Stream contentStream, string name)
        {
            Attachments.Add(new Attachment(contentStream, name));
        }

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="contentStream">A readable System.IO.Stream that contains the content for this attachment.</param>
        /// <param name="contentType">A System.Net.Mime.ContentType that describes the data in stream.</param>
        public void AddAttachment(Stream contentStream, ContentType contentType)
        {
            Attachments.Add(new Attachment(contentStream, contentType));
        }

        /// <summary>
        /// Adds an attachment to this message.
        /// </summary>
        /// <param name="contentStream">A readable System.IO.Stream that contains the content for this attachment.</param>
        /// <param name="name">A System.String that contains the value for the System.Net.Mime.ContentType.Name property of the System.Net.Mime.ContentType associated with this attachment. This value can be null.</param>
        /// <param name="mediaType">A System.String that contains the MIME Content-Header information for this attachment. This value can be null.</param>
        public void AddAttachment(Stream contentStream, string name, string mediaType)
        {
            Attachments.Add(new Attachment(contentStream, name, mediaType));
        }

        #endregion
        
        /// <summary>
        /// Adds a template to the email for rendering.
        /// </summary>
        /// <param name="template">A System.String that contains the template to be added. Can be an assembly resource name, a file path or an html string.</param>
        public void AddTemplate(string template)
        {
            // Is this template an embedded resource?
            var assembly = Assembly.GetCallingAssembly();
            if (assembly.GetManifestResourceNames().Contains(string.Format("{0}.{1}", assembly.GetName().Name, template)))
            {
                Templates.Add(new EmbeddedResourceTemplate(template, assembly));
                return;
            }

            // Is this template a file resource?
            if (File.Exists(template))
            {
                Templates.Add(new FileTemplate(template));
                return;
            }

            // It must be a basic string template
            Templates.Add(new StringTemplate(template));
        }

        ///<summary>
        ///Renders the Razor templates and returns the result as a System.String.
        ///</summary>
        ///<remarks></remarks>
        public string Render()
        {
            string output = string.Empty;

            // Build the final template from all templates
            using (var stream = new MemoryStream())
            {
                Templates.ForEach(x => x.Write(stream));

                stream.Position = 0;
                output = new StreamReader(stream).ReadToEnd();
            }

            // Parse the template using RazorEngine and replace all links with 
            return Razor.Parse(output, Set);
        }

    }

}