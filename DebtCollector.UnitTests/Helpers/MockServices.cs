using DebtCollector.Application.Common.Interfaces;
using Moq;

namespace DebtCollector.UnitTests.Helpers
{
    public static class MockServices
    {
        public static Mock<ICurrentUserService> CreateMockCurrentUserService(int? personId = 1)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.GetCurrentPersonId()).Returns(personId);
            return mock;
        }

        public static Mock<ITokenService> CreateMockTokenService()
        {
            var mock = new Mock<ITokenService>();
            mock.Setup(x => x.GenerateJwtToken(It.IsAny<Domain.Entities.Person>()))
                .Returns("mock-jwt-token");
            mock.Setup(x => x.GenerateRefreshToken())
                .Returns("mock-refresh-token");
            mock.Setup(x => x.GenerateUniqueRandomPasswordAsync(It.IsAny<int>()))
                .ReturnsAsync("ABCDE");
            return mock;
        }

        public static Mock<IEmailService> CreateMockEmailService()
        {
            var mock = new Mock<IEmailService>();
            mock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            return mock;
        }

        public static Mock<IBlobStorageService> CreateMockBlobStorageService()
        {
            var mock = new Mock<IBlobStorageService>();
            mock.Setup(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync("https://mock-blob-url.com/image.jpg");
            mock.Setup(x => x.DeleteImageAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            return mock;
        }
    }
}
