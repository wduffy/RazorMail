using System;
using System.Net.Mail;
using RazorMail.Parsers;
using RazorMail.Properties;

namespace RazorMail
{
    
    /// <summary>
    /// Allows applications to send razor templated e-mails by using the Simple Mail Transfer Protocol (SMTP)
    /// </summary>
    public class RazorMailSender : IRazorMailSender
    {
        private MailAddress Sender { get; set; }
        private Uri BaseUri { get; set; }
        private SmtpClient Client { get; set; }
        private IParser Parser { get; set; }

        public RazorMailSender(MailAddress sender, Uri baseUri) : this(sender, baseUri, null, null) { }
        public RazorMailSender(MailAddress sender, Uri baseUri, SmtpClient client) : this(sender, baseUri, client, null) { }
        public RazorMailSender(MailAddress sender, Uri baseUri, SmtpClient client, IParser parser)
        {
            Sender = sender;
            BaseUri = baseUri;
            Client = client ?? new SmtpClient("127.0.0.1");
            Parser = parser ?? new HtmlAgilityPackParser(baseUri);
        }

#region IRazorMailSender Members

        /// <summary>
        /// Sends the specified RazorMailMessage to an SMTP server for delivery
        /// </summary>
        /// <param name="message"></param>
        public void Send(RazorMailMessage razorMailMessage)
        {
            using (var mailMessage = GetMailMessage(razorMailMessage))
                Client.Send(mailMessage);
        }

#endregion

        protected virtual MailMessage GetMailMessage(RazorMailMessage razorMailMessage)
        {
            var mailMessage = razorMailMessage.GetMailMessage(Parser);

            // Set the sender and, if required, the from address
            mailMessage.Sender = Sender;                                // Sender (e.g. the CEO's Assistant)
            mailMessage.From = mailMessage.From ?? Sender;              // From (e.g. the CEO)
            
            return mailMessage;
        }

    }
}