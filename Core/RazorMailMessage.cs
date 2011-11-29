using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using RazorEngine;
using RazorMail.Parsers;
using RazorMail.Properties;
using RazorMail.Templates;

namespace RazorMail
{
    using System.Text;

    /// <summary>
    /// Represents a razor e-mail message that can be sent using a concrete implementation of the RazorMail.IRazorMailSender interface.
    /// </summary>
    public class RazorMailMessage : IDisposable
    {

        protected MailMessage Message { get; set; }
        //public string Subject { get { return Message.Subject; } set { Message.Subject = value; } }
        public Encoding Encoding { get; set; }
        public MailPriority Priority { get { return Message.Priority; } set { Message.Priority = value; } }
        public MailAddress From { get { return Message.From; } set { Message.From = value; } }
        public MailAddressCollection To { get { return Message.To; } }
        public MailAddressCollection Cc { get { return Message.CC; } }
        public MailAddressCollection Bcc { get { return Message.Bcc; } }
        public MailAddressCollection ReplyTo { get { return Message.ReplyToList; } }
        public AttachmentCollection Attachments { get { return Message.Attachments; } }
        
        public string PlainText { get; set; }
        public TemplateCollection Templates { get; private set; }
        public dynamic Set { get; private set; }

        /// <summary>
        /// Initialises an empty instance of the RazorMail.RazorMailmessage class.
        /// </summary>
        /// <param name="subject">A System.String that contains the subject text.</param>
        public RazorMailMessage(string subject) : this(subject, Assembly.GetCallingAssembly(), null) { }

        /// <summary>
        /// Initialises an empty instance of the RazorMail.RazorMailmessage class.
        /// </summary>
        /// <param name="subject">A System.String that contains the subject text.</param>
        /// <param name="assembly">A reference to the System.Reflection.Assembly that contains embedded resources used within this RazorMailMessage's template</param>
        public RazorMailMessage(string subject, Assembly assembly) : this(subject, assembly, null) { }
        
        internal RazorMailMessage(string subject, Assembly assembly, IFileReader fileReader)
        {
            Message = new MailMessage();

            Message.Subject = subject;
            Encoding = Encoding.UTF8;

            Templates = new TemplateCollection(assembly, fileReader ?? new FileReader());
            Set = new ExpandoObject();
        }

        internal virtual MailMessage GetMailMessage(IParser parser)
        {            
            // Perform requirement checks
            EnsureValues();

            // Set econding for headers and subject
            Message.HeadersEncoding = Encoding;
            Message.SubjectEncoding = Encoding;

            // Create the plain text and html views
            CreateAlternateViews(parser).ForEach(x => Message.AlternateViews.Add(x));
            
            // Return the MailMessage
            return Message;
        }

        protected void EnsureValues()
        {
            if (string.IsNullOrWhiteSpace(Message.Subject)) throw new Exception(Resources.ExceptionNoSubject);
            if (Templates.IsEmpty()) throw new Exception(Resources.ExceptionNoTemplates);
            if (To.IsEmpty()) throw new Exception(Resources.ExceptionNoRecipient);
        }

        protected IEnumerable<AlternateView> CreateAlternateViews(IParser parser)
        {
            var views = new List<AlternateView>();
            
            // Get the rendered template and base all its relative urls
            var body = parser.BaseUrls(RenderTemplates());

            // Create the plain text view (base64)
            var plainTextView = AlternateView.CreateAlternateViewFromString(PlainText ?? parser.StripHtml(body), Encoding, "text/plain");
            views.Add(plainTextView);

            // Create the html view (base64)
            var htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding, "text/html");
            views.Add(htmlView);

            // Return the alternate views
            return views;
        }

        ///<summary>
        ///Renders the Razor templates and returns the result as a System.String.
        ///</summary>
        ///<remarks></remarks>
        protected string RenderTemplates()
        {
            string output = string.Empty;
            
            // Build the final template from all templates
            using (var stream = new MemoryStream())
            {
                Templates.ForEach(x => x.Write(stream));

                stream.Position = 0;
                output = new StreamReader(stream).ReadToEnd();
            }
                        
            // Parse the template using RazorEngine and return the result
            return Razor.Parse(output, Set);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Message.Dispose();
            // This object will be cleaned up by the Dispose method.
            // Therefore, call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        
        #endregion

    }

}