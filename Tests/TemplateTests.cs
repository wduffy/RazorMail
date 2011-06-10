using System.IO;
using System.Reflection;
using System.Text;
using Moq;
using NUnit.Framework;
using RazorMail.Templates;

namespace RazorMail.Tests
{

    [TestFixture]
    public class TemplateTests
    {

        [Test]
        public void EmbeddedResourceTemplate_CanWriteContentToStream()
        {
            var test = new RazorMailMessage("TESTING");
            
            // Arrange
            var result = string.Empty;
            var stream = new MemoryStream();
            var template = new EmbeddedResourceTemplate(Assembly.GetExecutingAssembly(), "Resources.Test.html");

            // Act
            template.Write(stream);
            stream.Position = 1;
            using (var sr = new StreamReader(stream))
                result = sr.ReadToEnd();

            // Assert
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void StringTemplate_CanWriteContentToStream()
        {
            // Arrange
            var result = string.Empty;
            var stream = new MemoryStream();
            var template = new StringTemplate("String template can write to stream.");

            // Act
            template.Write(stream);
            stream.Position = 1;
            using (var sr = new StreamReader(stream))
                result = sr.ReadToEnd();

            // Assert
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void FileTemplate_CanWriteContentToStream()
        {
            // Arrange
            var file = new MemoryStream(Encoding.UTF8.GetBytes("File template can write to stream."));
            var reader = new Mock<IFileReader>();
            reader.Setup(x => x.OpenRead(It.IsAny<string>())).Returns(file);

            // Arrange
            var result = string.Empty;
            var stream = new MemoryStream();
            var template = new FileTemplate(reader.Object, "C:\\files\\test.html");

            // Act
            template.Write(stream);
            stream.Position = 1;
            using (var sr = new StreamReader(stream))
                result = sr.ReadToEnd();

            // Assert
            Assert.That(result, Is.Not.Empty);
        }

    }

}