var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IAdPlatformService, AdPlatformService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapPost("/api/adplatforms/upload", (FilePathRequest request, IAdPlatformService service)
    => service.UploadPlatforms(request.FilePath));

app.MapGet("/api/adplatforms/search", (string location, IAdPlatformService service)
    => service.SearchPlatforms(location));


app.Run();