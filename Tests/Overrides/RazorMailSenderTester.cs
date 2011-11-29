using System;
using System.Net.Mail;

namespace RazorMail.Tests
{
    public class RazorMailSenderTester : RazorMailSenderTestable
    {
        public RazorMailSenderTester(MailAddress sender, Uri uri, string testing) : base(sender, uri, testing) { }
        public RazorMailSenderTester(MailAddress sender, Uri uri, string testing, SmtpClient client) : base(sender, uri, testing, client) { }

        public MailMessage TestGetMailMessage(RazorMailMessage message)
        {
            return base.GetMailMessage(message);
        }

        public MailAddress TestGetTestingMailAddress(string address)
        {
            return base.GetTestingMailAddress(address);
        }

    }
}
