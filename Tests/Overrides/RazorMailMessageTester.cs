using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using RazorMail.Parsers;

namespace RazorMail.Tests
{
    public class RazorMailMessageTester : RazorMailMessage
    {
        public RazorMailMessageTester(string subject) : base(subject) { }
        public RazorMailMessageTester(string subject, Assembly assembly) : base(subject, assembly) { }

        public MailMessage InnerMessage { get { return Message; } }

        public void TestEnsureValues()
        {
            base.EnsureValues();
        }

        public string TestRenderTemplates()
        {
            return base.RenderTemplates();
        }

        public IEnumerable<AlternateView> TestCreateAlternateViews(IParser parser)
        {
            return base.CreateAlternateViews(parser);
        }

    }
}
