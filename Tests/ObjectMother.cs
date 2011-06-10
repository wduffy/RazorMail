using System.Linq;
using System.Net.Mail;

namespace RazorMail.Tests
{
    public static class ObjectMother
    {
        public static MailMessage Message { get; private set; }
        public static string Subject { get { return Message.Subject; } }
        public static MailAddress Sender { get { return Message.Sender; } }
        public static MailAddress From { get { return Message.From; } }
        public static MailAddress To { get { return Message.To.First(); } }
        public static MailAddress Cc { get { return Message.CC.First(); } }
        public static MailAddress Bcc { get { return Message.Bcc.First(); } }
        public static MailAddress ReplyTo { get { return Message.ReplyToList.First(); } }
        public static string TestAddress { get; private set; }

        static ObjectMother()
        {
            Message = new MailMessage();
            Message.Subject = "RazorMailMessage currently under test.";
            Message.Sender = new MailAddress("sender@wduffy.co.uk", "Sender");
            Message.From = new MailAddress("from@wduffy.co.uk", "From");
            Message.To.Add(new MailAddress("to@wduffy.co.uk", "To"));
            Message.CC.Add(new MailAddress("cc@wduffy.co.uk", "Carbon Copy"));
            Message.Bcc.Add(new MailAddress("bcc@wduffy.co.uk", "Blind Carbon Copy"));
            Message.ReplyToList.Add(new MailAddress("replyto@wduffy.co.uk", "Reply To"));
            TestAddress = "test@wduffy.co.uk";
        }
    }
}
