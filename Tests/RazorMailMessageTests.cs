using System.Linq;
using System.Net.Mail;
using System.Text;
using Moq;
using NUnit.Framework;
using RazorMail.Parsers;

namespace RazorMail.Tests
{

    [TestFixture]
    public class RazorMailMessageTests
    {

        [Test]
        public void Constructor_CanCallWithSubjectOnly()
        {
            // Arrange
            RazorMailMessageTester message = null;

            // Act
            message = new RazorMailMessageTester(ObjectMother.Subject);

            // Assert
            Assert.That(message.InnerMessage.Subject, Is.EqualTo(ObjectMother.Subject));
        }

        [Test]
        public void Constructor_CanCallWithSubjectAndAndAssembly()
        {
            // Arrange
            RazorMailMessageTester message = null;

            // Act
            message = new RazorMailMessageTester(ObjectMother.Subject, System.Reflection.Assembly.GetExecutingAssembly());

            // Assert
            Assert.That(message.InnerMessage.Subject, Is.EqualTo(ObjectMother.Subject));
        }

        [Test]
        public void Constructor_CanInitialiseDefaults()
        {
            // Arrange
            RazorMailMessageTester message = null;

            // Act
            message = new RazorMailMessageTester(ObjectMother.Subject, System.Reflection.Assembly.GetExecutingAssembly());

            // Assert
            Assert.That(message.InnerMessage, Is.Not.Null);
            Assert.That(message.InnerMessage.Subject, Is.Not.Null); 
            Assert.That(message.Encoding, Is.EqualTo(Encoding.UTF8));
            Assert.That(message.Templates, Is.Not.Null);
            Assert.That(message.Set, Is.Not.Null);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "RazorMail: The subject cannot be null or empty.")]
        public void TestEnsureValues_WithNoSubject_ThrowsException()
        {
            // Arrange
            var message = new RazorMailMessageTester("     ");
            
            // Act
            message.TestEnsureValues();

            // Assert
            // Should throw exception
        }

        [Test]
        [ExpectedException(ExpectedMessage = "RazorMail: At least one template must be specified before sending email.")]
        public void TestEnsureValues_WithNoTemplates_ThrowsException()
        {
            // Arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // Act
            message.TestEnsureValues();

            // Assert
            // Should throw exception
        }

        [Test]
        [ExpectedException(ExpectedMessage = "RazorMail: At least one recipient must be specified before sending email.")]
        public void TestEnsureValues_WithNoRecipients_ThrowsException()
        {
            // Arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);
            message.Templates.Add("Test Template");

            // Act
            message.TestEnsureValues();

            // Assert
            // Should throw exception
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetFromAddress()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.From = ObjectMother.From;

            // assert
            Assert.That(message.From, Is.EqualTo(ObjectMother.From));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetToRecipients()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);
           
            // act
            message.To.Add(ObjectMother.To);

            // assert
            Assert.That(message.To, Has.Member(ObjectMother.To));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetCcRecipients()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.Cc.Add(ObjectMother.Cc);

            // assert
            Assert.That(message.Cc, Has.Member(ObjectMother.Cc));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetBccRecipients()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.Bcc.Add(ObjectMother.Bcc);

            // assert
            Assert.That(message.Bcc, Has.Member(ObjectMother.Bcc));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetReplyToRecipients()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.ReplyTo.Add(ObjectMother.ReplyTo);

            // assert
            Assert.That(message.ReplyTo, Has.Member(ObjectMother.ReplyTo));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetAttachments()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);
            var attachment = Attachment.CreateAttachmentFromString("This is a test attachment", "text/plain");

            // act
            message.Attachments.Add(attachment);

            // assert
            Assert.That(message.Attachments, Has.Member(attachment));
        }

        [Test]
        public void RazorMailMessage_CanSetAndGetPriority()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);
            var priority = MailPriority.High;

            // act
            message.Priority = priority;

            // assert
            Assert.That(message.Priority, Is.EqualTo(priority));
        }

        [Test]
        public void CreateAlternateViews_DoesCallIParserMethods()
        {
            // arrange
            var template = "Template";
            var based = "Based Urls";
            var plaintext = "Stripped Html";
 
            var parser = new Mock<IParser>();
            var message = new RazorMailMessageTester(ObjectMother.Subject);
            
            message.Templates.Add(template); 
            parser.Setup(x => x.BaseUrls(template)).Returns(based);
            parser.Setup(x => x.StripHtml(based)).Returns(plaintext);
            
            // act
            var result = message.TestCreateAlternateViews(parser.Object);

            // assert
            parser.Verify(x => x.BaseUrls(template));
            parser.Verify(x => x.StripHtml(based));
        }

        [Test]
        public void CreateAlternateViews_DoesReturnPlainTextAndHtmlAlternateViews()
        {
            // arrange
            var parser = new Mock<IParser>();
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            parser.Setup(x => x.BaseUrls(It.IsAny<string>())).Returns<string>(x => x);
            parser.Setup(x => x.StripHtml(It.IsAny<string>())).Returns<string>(x => x);
            message.Templates.Add("Template");

            // act
            var result = message.TestCreateAlternateViews(parser.Object);

            // assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.ElementAt(0).ContentType, Is.EqualTo(new System.Net.Mime.ContentType("text/plain; charset=utf-8")));
            Assert.That(result.ElementAt(1).ContentType, Is.EqualTo(new System.Net.Mime.ContentType("text/html; charset=utf-8")));
        }

        [Test]
        public void RenderTemplate_CanReturnRenderedTemplatesAsString()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.Templates.Add("This is a test template by @Model.Name.");
            message.Set.Name = "William Duffy";
            var result = message.TestRenderTemplates();

            // assert
            Assert.That(result, Is.EqualTo("This is a test template by William Duffy."));
        }

        [Test]
        public void GetMailMessage_CanReturnMailMessage()
        {
            // arrange
            var parser = new Mock<IParser>();
            var message = new RazorMailMessage(ObjectMother.Subject);
            parser.Setup(x => x.BaseUrls(It.IsAny<string>())).Returns<string>(x => x);
            parser.Setup(x => x.StripHtml(It.IsAny<string>())).Returns<string>(x => x);

            // act
            message.To.Add(ObjectMother.To);
            message.Templates.Add("Test template");
            var result = message.GetMailMessage(parser.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<MailMessage>());
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Cannot access a disposed object.\r\nObject name: 'System.Net.Mail.MailMessage'.")]
        public void Dispose_DoesDisposeOfInnerMailMessage()
        {
            // arrange
            var message = new RazorMailMessageTester(ObjectMother.Subject);

            // act
            message.Dispose();

            // assert
            Assert.That(message.InnerMessage.Attachments, Is.Null);
        }

    }

}