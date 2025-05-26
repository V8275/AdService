using Microsoft.AspNetCore.Http.HttpResults;

namespace AdServiceTest
{
    public class UnitTest1
    {
        [Fact]
        public void UploadPlatforms_ValidFile_Success()
        {
            var service = new AdPlatformService();
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Google:US");

            var result = service.UploadPlatforms(tempFile);

            Assert.IsType<Ok<string>>(result);
            File.Delete(tempFile);
        }

        [Fact]
        public void SearchPlatforms_ExistingLocation_ReturnsPlatform()
        {
            var service = new AdPlatformService();

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Google:US,UK\nFacebook:FR,ES");

            service.UploadPlatforms(tempFile);
            var result = service.SearchPlatforms("US");

            var platforms = Assert.IsType<Ok<List<string>>>(result).Value;
            Assert.NotEmpty(platforms);
            Assert.Contains("Google", platforms);

            File.Delete(tempFile);
        }


        [Fact]
        public void UploadPlatforms_InvalidFile_ReturnsError()
        {
            var service = new AdPlatformService();
            var result = service.UploadPlatforms("nonexistent.txt");

            Assert.IsType<NotFound<string>>(result);
        }
    }
}
