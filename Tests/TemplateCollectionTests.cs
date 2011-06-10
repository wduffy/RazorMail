using System.Reflection;
using Moq;
using NUnit.Framework;
using RazorMail.Templates;

namespace RazorMail.Tests
{
    
    public class TemplateCollectionTests
    {

        [Test]
        [ExpectedException(ExpectedMessage = "RazorMail: Parameter cannot be null or empty.\r\nParameter name: assembly")]
        public void TemplateCollectionConstructor_WithNoAssembly_ThrowsException()
        {
            // Arrange
            var reader = new Mock<IFileReader>();

            // Act
            var collection = new TemplateCollection(null, reader.Object);

            // Assert
            // Exception thrown
        }

        [Test]
        [ExpectedException(ExpectedMessage = "RazorMail: Parameter cannot be null or empty.\r\nParameter name: fileReader")]
        public void TemplateCollectionConstructor_WithNoFileReader_ThrowsException()
        {
            // Arrange
            IFileReader reader = null;

            // Act
            var collection = new TemplateCollection(Assembly.GetExecutingAssembly(), reader);

            // Assert
            // Exception thrown
        }

        [Test]
        public void Add_WithEmbeddedResource_CreatesFileTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var collection = new TemplateCollection(Assembly.GetExecutingAssembly(), reader.Object);
            
            // Act
            collection.Add("Resources.Test.html");

            // Assert
            Assert.That(collection, Has.Some.TypeOf<EmbeddedResourceTemplate>());
        }

        [Test]
        public void Add_WithFile_CreatesFileTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var collection = new TemplateCollection(Assembly.GetExecutingAssembly(), reader.Object);
            reader.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            // Act
            collection.Add("C:\\Files\\Test.html");

            // Assert
            Assert.That(collection, Has.Some.TypeOf<FileTemplate>());
        }

        [Test]
        public void Add_WithString_CreatesStringTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var collection = new TemplateCollection(Assembly.GetExecutingAssembly(), reader.Object);

            // Act
            collection.Add("This is a text template");

            // Assert
            Assert.That(collection, Has.Some.TypeOf<StringTemplate>());
        }

        [Test]
        public void GetEnumerator_ReturnsEnumerator()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var collection = new TemplateCollection(Assembly.GetExecutingAssembly(), reader.Object);

            // Act
            var result = collection.GetEnumerator();

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }

}