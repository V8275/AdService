public interface IAdPlatformService
{
    public IResult UploadPlatforms(string? filePath);
    public IResult SearchPlatforms(string location);
    public List<AdPlatform> ParseLines(string[] lines);
}