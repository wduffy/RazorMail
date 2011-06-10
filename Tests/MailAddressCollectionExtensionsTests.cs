using System.Net.Mail;
using NUnit.Framework;

namespace RazorMail.Tests
{
    
    public class AddressCollectionTests
    {

        [Test]
        public void Add_WithAddressAndDisplayName_DoesAdd()
        {
            // Arrange
            var collection = new MailAddressCollection();

            // Act
            collection.Add(ObjectMother.To.Address, ObjectMother.To.DisplayName);

            // Assert
            Assert.That(collection, Has.Member(ObjectMother.To));
        }

    }

}