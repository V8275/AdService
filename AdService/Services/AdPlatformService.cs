using System.Collections.Concurrent;

public class AdPlatformService : IAdPlatformService
{
    private readonly ConcurrentDictionary<string, AdPlatform> _platforms = new();

    public AdPlatformService() { }

    public IResult UploadPlatforms(string? filePath)
    {
        try
        {
            if (filePath == null)
                return Results.BadRequest("FilePath is required.");

            if (!File.Exists(filePath))
                return Results.NotFound($"File not found at {filePath}");

            var lines = System.IO.File.ReadAllLines(filePath);
            var platforms = lines.Select(line =>
            {
                var parts = line.Split(':');

                if (parts.Length != 2)
                    throw new FormatException($"Invalid line format: {line}");

                return new AdPlatform
                {
                    Name = string.IsNullOrWhiteSpace(parts[0])
                        ? throw new FormatException("Platform name cannot be empty")
                        : parts[0].Trim(),

                    Locations = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(loc => loc.Trim())
                        .Where(loc => !string.IsNullOrWhiteSpace(loc))
                        .ToList()
                };
            });

            var newPlatforms = new ConcurrentDictionary<string, AdPlatform>();
            foreach (var platform in platforms)
            {
                if (platform.Name == null)
                    continue;

                newPlatforms.TryAdd(platform.Name, platform);
            }

            _platforms.Clear();
            foreach (var kvp in newPlatforms)
            {
                _platforms.TryAdd(kvp.Key, kvp.Value);
            }

            return Results.Ok("Ad platforms uploaded successfully.");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error uploading ad platforms: {ex.Message}");
        }
    }

    public IResult SearchPlatforms(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return Results.BadRequest("Location is required.");

        try
        {
            var result = _platforms.Values
                .Where(platform => platform.Locations?.Any(loc =>
                    !string.IsNullOrEmpty(loc) &&
                    !string.IsNullOrEmpty(location) &&
                    location.StartsWith(loc, StringComparison.OrdinalIgnoreCase)) ?? false)
                .Select(platform => platform.Name)
                .Where(name => name != null)
                .ToList();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error searching ad platforms: {ex.Message}");
        }
    }

    public List<AdPlatform> ParseLines(string[] lines)
    {
        return lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split(':', 2);
                if (parts.Length != 2)
                    throw new FormatException($"Invalid line format: {line}");

                return new AdPlatform
                {
                    Name = parts[0].Trim(),
                    Locations = parts[1].Split(',')
                        .Select(loc => loc.Trim())
                        .Where(loc => !string.IsNullOrEmpty(loc))
                        .ToList()
                };
            })
            .ToList();
    }
}
