using LinkPulseDefinitions;
using LinkPulseImplementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseTests
{
    public class ShortenerControllerTests
    {
        [Fact]
        public void TryAddNewURL_SuccessfullyShortensUrl()
        {
            // Arrange
            var mockHashProvider = new Mock<IHashProvider>();
            mockHashProvider.Setup(mock => mock.Hash(It.IsAny<string>())).Returns("shortened");

            var mockStorage = new Mock<IStorage>();
            mockStorage.Setup(mock => mock.TryAddKeyValuePair(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(true);

            var mockConfiguration = new ConfigurationManager();
            mockConfiguration.AddInMemoryCollection(new Dictionary<string, string?> { ["Shortener:ExpirationTimeSeconds"] = "60" });

            var mockLogger = new Mock<ILogger<ShortenerController>>();

            IShortenerController shortenerController = new ShortenerController(mockHashProvider.Object, mockStorage.Object, mockConfiguration, mockLogger.Object);

            // Act
            var result = shortenerController.TryAddNewURL("http://example.com", out var shortenedUrl);

            // Assert
            result.ShouldBeTrue();
            shortenedUrl.ShouldBe("shortened");
            mockStorage.Verify(st => st.TryAddKeyValuePair("shortened", "http://example.com", TimeSpan.FromSeconds(60)), Times.Once);
        }

        [Fact]
        public void TryGetURLByShortenedVersion_ReturnsTrueAndFullUrl()
        {
            // Arrange
            var mockStorage = new Mock<IStorage>();
            mockStorage.Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<string?>.IsAny, It.IsAny<TimeSpan?>())).Callback((string key, out string value, TimeSpan? exp) => { value = "http://example.com"; }).Returns(true);

            var mockConfiguration = new ConfigurationManager();
            mockConfiguration.AddInMemoryCollection(new Dictionary<string, string?> { ["Shortener:ExpirationTimeSeconds"] = "60" });

            var mockLogger = new Mock<ILogger<ShortenerController>>();


            IShortenerController shortenerController = new ShortenerController(null!, mockStorage.Object, mockConfiguration, mockLogger.Object);

            // Act
            var result = shortenerController.TryGetURLByShortenedVersion("shortened", out var fullUrl);

            // Assert
            result.ShouldBeTrue();
            fullUrl.ShouldBe("http://example.com");
            mockStorage.Verify(st => st.TryGetValue("shortened", out It.Ref<string?>.IsAny, It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public void TryGetURLByShortenedVersion_ReturnsFalseIfNotFound()
        {
            // Arrange
            var mockStorage = new Mock<IStorage>();
            mockStorage.Setup(mock => mock.TryGetValue(It.IsAny<string>(), out It.Ref<string?>.IsAny, It.IsAny<TimeSpan?>())).Callback((string key, out string? value, TimeSpan? exp) => { value = null; }).Returns(false);

            var mockConfiguration = new ConfigurationManager();
            mockConfiguration.AddInMemoryCollection(new Dictionary<string, string?> { ["Shortener:ExpirationTimeSeconds"] = "60" });

            var mockLogger = new Mock<ILogger<ShortenerController>>();


            IShortenerController shortenerController = new ShortenerController(null!, mockStorage.Object, mockConfiguration, mockLogger.Object);

            // Act
            var result = shortenerController.TryGetURLByShortenedVersion("shortened", out var fullUrl);

            // Assert
            result.ShouldBeFalse();
            fullUrl.ShouldBeNull();
            mockStorage.Verify(st => st.TryGetValue("shortened", out It.Ref<string?>.IsAny, It.IsAny<TimeSpan?>()), Times.Once);
        }

    }
}
