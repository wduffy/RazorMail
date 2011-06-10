using System.Reflection;
using Moq;
using NUnit.Framework;
using RazorMail.Templates;

namespace RazorMail.Tests
{

    [TestFixture]
    public class TemplateFactoryTests
    {

        [Test]
        public void Create_WithEmbeddedResourceString_GetsEmbeddedResourceTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var factory = new TemplateFactory(reader.Object, Assembly.GetExecutingAssembly());

            // Act
            var result = factory.Create("Resources.Test.html");

            // Assert
            Assert.That(result, Is.InstanceOf<EmbeddedResourceTemplate>());
        }

        [Test]
        public void Create_WithFilePath_GetsFileTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>(); reader.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            var factory = new TemplateFactory(reader.Object, Assembly.GetExecutingAssembly());
            
            // Act
            var result = factory.Create("C:\\files\template.html");

            // Assert
            Assert.That(result, Is.InstanceOf<FileTemplate>());
        }

        [Test]
        public void Create_WithString_GetsStringTemplate()
        {
            // Arrange
            var reader = new Mock<IFileReader>();
            var factory = new TemplateFactory(reader.Object, Assembly.GetExecutingAssembly());

            // Act
            var result = factory.Create("This is a plain string template.");

            // Assert
            Assert.That(result, Is.InstanceOf<StringTemplate>());
        }

    }

}