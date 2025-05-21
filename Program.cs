using Microsoft.AspNetCore.Mvc;
using TweetScreenshotApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TweetScreenshotService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Football API");
        c.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

app.MapPost("/tweet-screenshot", async (
        [FromBody] TweetUrl url,
        [FromServices] TweetScreenshotService tweetScreenshotService) =>
    {
        var data = await tweetScreenshotService.CaptureTweetImageAsync(url.Url);
        return Results.File(data, "image/png");
    })
    .WithName("GetTweetScreenshotPuppeteer")
    .WithOpenApi();

await app.RunAsync().ConfigureAwait(true);


public record TweetUrl(string Url);