using System;
using System.Net.Mail;

namespace RazorMail.Tests
{
    public class RazorMailSenderWithTestEmailTester : RazorMailSenderWithTestEmail
    {
        public RazorMailSenderWithTestEmailTester(MailAddress sender, Uri uri, string testing) : base(sender, uri, testing) { }
        public RazorMailSenderWithTestEmailTester(MailAddress sender, Uri uri, string testing, SmtpClient client) : base(sender, uri, testing, client) { }

        public MailAddress TestTestAddress { get { return TestAddress; } }

        public MailMessage TestGetMailMessage(RazorMailMessage message)
        {
            return base.GetMailMessage(message);
        }
    }
}
