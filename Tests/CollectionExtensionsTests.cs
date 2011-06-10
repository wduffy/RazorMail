using System.Collections.Generic;
using NUnit.Framework;

namespace RazorMail.Tests
{

    [TestFixture]
    public class CollectionExtensionsTests
    {

        [Test]
        public void ForEach_WithIEnumerable_PerformsAction()
        {
            // Arrange
            var values = new[] { "William", "Sarah", "Abigail", "Jessica" };

            //Act
            var result = string.Empty;
            values.ForEach(x => result += x);

            // Assert
            Assert.That(result, Is.EqualTo(string.Join(string.Empty, values)));
        }

        [Test]
        public void ForEach_WithNull_ReturnsWithoutException()
        {
            // Arrange
            IEnumerable<string> values = null;

            //Act
            var result = string.Empty;
            values.ForEach(x => result += x);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void IsEmpty_WithEmptyIEnumerable_ReturnsTrue()
        {
            // Arrange
            IEnumerable<string> values = null;

            //Act
            var result = values.IsEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmpty_WithPopulatedIEnumerable_ReturnsFalse()
        {
            // Arrange
            var values = new[] { "William", "Sarah", "Abigail", "Jessica" };

            //Act
            var result = values.IsEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

    }

}
