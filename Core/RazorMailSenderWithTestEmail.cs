using System;
using System.Net.Mail;
using RazorMail.Parsers;
using RazorMail.Properties;

namespace RazorMail
{
    
    /// <summary>
    /// Allows applications to send razor templated e-mails by using the Simple Mail Transfer Protocol (SMTP). Constructor
    /// can take a testEmail address. If supplied all emails are sent to this address. Useful for testing via a .config setting.
    /// I.e. new RazorMailSenderWithTestEmail("sender", "baseUri", ConfigurationManager.AppSettings["TestEmail"])
    /// </summary>
    public class RazorMailSenderWithTestEmail : RazorMailSender
    {

        protected MailAddress TestAddress { get; set; }

        public RazorMailSenderWithTestEmail(MailAddress sender, Uri baseUri) : this(sender, baseUri, null, null) { }
        public RazorMailSenderWithTestEmail(MailAddress sender, Uri baseUri, string testEmail) : this(sender, baseUri, testEmail, null) { }
        public RazorMailSenderWithTestEmail(MailAddress sender, Uri baseUri, string testEmail, SmtpClient client) : base(sender, baseUri, client)
        {
            TestAddress = !string.IsNullOrWhiteSpace(testEmail)
                ? new MailAddress(testEmail, Resources.TestingRecipientName)
                : null;
        }

        protected override MailMessage GetMailMessage(RazorMailMessage razorMailMessage)
        {
            var mailMessage = base.GetMailMessage(razorMailMessage);

            // If in testing then alter message
            if (TestAddress != null)
            {
                mailMessage.To.Clear(); // TODO: Put the original recipient lists into the test message (inline or attachment?)
                mailMessage.CC.Clear();
                mailMessage.Bcc.Clear();

                mailMessage.To.Add(TestAddress);
            }

            return mailMessage;
        }
        
    
    }
}