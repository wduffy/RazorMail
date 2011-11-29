using System;
using System.Linq;
using NUnit.Framework;
using RazorMail.Properties;
using Moq;
using System.Net.Mail;
using RazorMail.Parsers;

namespace RazorMail.Tests
{

    [TestFixture]
    public class RazorMailSenderTests
    {

        [Test]
        [ExpectedException(ExpectedException = typeof(SmtpException), ExpectedMessage = "Failure sending mail.")]
        public void Send_DoesCallSendOnSmtpClient()
        {
            // Arrange
            var sender = new RazorMailSenderTestable(ObjectMother.Sender, null);
            var message = new Mock<RazorMailMessage>(ObjectMother.Subject);
            message.Setup(x => x.GetMailMessage(It.IsAny<IParser>())).Returns(ObjectMother.Message);

            // Act
            sender.Send(message.Object);

            // Assert
            // Exception should be thrown by the SmtpClient object.
            // It's a nasty way to verify that SmtpClient.Send() is called, but I dont want to overcomplicate the surface with an SmtpClient wrapper.
        }

        [Test]
        public void GetMailMessage_WithFromAddress_DoesOnlySetSender()
        {
            // Arrange
            var sender = new RazorMailSenderTester(ObjectMother.Sender, null, ObjectMother.TestAddress);
            var message = new Mock<RazorMailMessage>(ObjectMother.Subject);
            message.Setup(x => x.GetMailMessage(It.IsAny<IParser>())).Returns(ObjectMother.Message);

            // Act
            var result = sender.TestGetMailMessage(message.Object);

            // Assert
            Assert.That(result.Sender, Is.EqualTo(ObjectMother.Sender));
            Assert.That(result.From, Is.Not.EqualTo(ObjectMother.Sender));
        }

        [Test]
        public void GetMailMessage_WithoutFromAddress_DoesSetSenderAndFrom()
        {
            // Arrange
            var sender = new RazorMailSenderTester(ObjectMother.Sender, null, ObjectMother.TestAddress);
            var message = new Mock<RazorMailMessage>(ObjectMother.Subject);
            message.Setup(x => x.GetMailMessage(It.IsAny<IParser>())).Returns(new MailMessage());

            // Act
            var result = sender.TestGetMailMessage(message.Object);
            
            // Assert
            Assert.That(result.Sender, Is.EqualTo(ObjectMother.Sender));
            Assert.That(result.From, Is.EqualTo(ObjectMother.Sender));
        }

        [Test]
        public void GetMailMessage_WithSystemInTesting_ReplacesRecipientListWithTestingAddress()
        {
            // Arrange
            var sender = new RazorMailSenderTester(ObjectMother.Sender, null, ObjectMother.TestAddress);
            var message = new Mock<RazorMailMessage>(ObjectMother.Subject);
            message.Setup(x => x.GetMailMessage(It.IsAny<IParser>())).Returns(ObjectMother.Message);

            // Act
            var result = sender.TestGetMailMessage(message.Object);

            // Assert
            Assert.That(result.CC, Has.Count.EqualTo(0));
            Assert.That(result.Bcc, Has.Count.EqualTo(0));
            Assert.That(result.To, Has.Count.EqualTo(1));
            Assert.That(result.To.First().Address, Is.EqualTo(ObjectMother.TestAddress));
        }

        [Test]
        public void GetTestingMailAddress_DoesReturnMailAddressWithTestRecipientName()
        {
            // Arrange
            var message = new RazorMailSenderTester(ObjectMother.Sender, null, null, null);

            // Act
            var result = message.TestGetTestingMailAddress(ObjectMother.TestAddress);

            // Assert
            Assert.That(result.DisplayName, Is.EqualTo(Resources.TestingRecipientName));
            Assert.That(result.Address, Is.EqualTo(ObjectMother.TestAddress));
        }

    }

}
