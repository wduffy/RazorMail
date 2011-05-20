using System;
using System.Net.Mail;
using RazorMail.Extensions;
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
        private MailAddress Testing { get; set; }
        private Uri BaseUri { get; set; }
        private SmtpClient Client { get; set; }
        private IParser Parser { get; set; }

        public RazorMailSender(MailAddress sender, Uri baseUri) : this(sender, baseUri, null) { }
        public RazorMailSender(MailAddress sender, Uri baseUri, string testing) : this(sender, baseUri, testing, new SmtpClient("127.0.0.1")) { }
        public RazorMailSender(MailAddress sender, Uri baseUri, string testing, SmtpClient client) : this(sender, baseUri, testing, client, new HtmlAgilityPackParser(baseUri)){ }
        public RazorMailSender(MailAddress sender, Uri baseUri, string testing, SmtpClient client, IParser parser)
        {
            Sender = sender;
            BaseUri = baseUri;
            Client = client;
            Parser = parser;

            if (!string.IsNullOrWhiteSpace(testing))
                Testing = new MailAddress(testing, Resources.TestingRecipientName);
        }

#region IMailSender Members

        /// <summary>
        /// Sends the specified RazorMailMessage to an SMTP server for delivery
        /// </summary>
        /// <param name="message"></param>
        public void Send(RazorMailMessage message)
        {
            var body = message.Render();

            // Perform requirement checks
            if (message.To.IsEmpty()) throw new Exception(Resources.ExceptionNoRecipient);
            if (string.IsNullOrWhiteSpace(message.Subject)) throw new Exception(Resources.ExceptionNoSubject);
            if (string.IsNullOrWhiteSpace(body)) throw new Exception(Resources.ExceptionNoBody);

            // Create and send the mail message
            using (var mail = new MailMessage())
            {
                mail.Priority = message.Priority;
                mail.Subject = message.Subject;
                mail.Sender = Sender;                                           // Sender (e.g. the CEO's Assistant)
                mail.From = message.From ?? Sender;                             // From (e.g. the CEO)
                message.ReplyTo.ForEach(x => mail.ReplyToList.Add(x));          // ReplyTo (e.g. the Secretary)
                message.To.ForEach(x => mail.To.Add(x));                        // Recipient
                message.Cc.ForEach(x => mail.CC.Add(x));                        // Cc
                message.Bcc.ForEach(x => mail.Bcc.Add(x));                      // Bcc
                message.Attachments.ForEach(x => mail.Attachments.Add(x));

                // If in testing then alter message
                if (Testing != null)
                {
                    // TODO: Put the original recipient lists into the test message (inline or attachment?)
                    mail.To.Clear();
                    mail.CC.Clear();
                    mail.Bcc.Clear();

                    mail.To.Add(Testing);
                }
                
                // Fully qualify all urls in the body
                body = Parser.BaseUrls(body);

                // Create and add the plain text view (base64)
                var plainTextView = AlternateView.CreateAlternateViewFromString(message.PlainText ?? Parser.StripHtml(body), message.Encoding, "text/plain");
                mail.AlternateViews.Add(plainTextView);

                // Create and add the html view (base64)
                var htmlView = AlternateView.CreateAlternateViewFromString(body, message.Encoding, "text/html");
                mail.AlternateViews.Add(htmlView);

                // Send the message
                Client.Send(mail);
            }
        }

#endregion
    }
}