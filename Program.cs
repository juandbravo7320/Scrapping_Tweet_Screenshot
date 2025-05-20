using Microsoft.AspNetCore.Mvc;
using TweetScreenshotApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TweetScreenshotService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/tweet-screenshot", async (
        [FromBody] TweetUrl url,
        [FromServices] TweetScreenshotService tweetScreenshotService) =>
    {
        var data = await tweetScreenshotService.CaptureTweetImageAsync2(url.Url);
        return Results.File(data, "image/png");
    })
    .WithName("GetTweetScreenshot")
    .WithOpenApi();

await app.RunAsync();


public record TweetUrl(string Url);