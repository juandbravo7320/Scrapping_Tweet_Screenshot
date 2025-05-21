using PuppeteerSharp;

namespace TweetScreenshotApp;

public class TweetScreenshotService
{
    
    public async Task<byte[]> CaptureTweetImageAsync(string tweetUrl)
    {   
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions
            {
                Headless = true,
                DefaultViewport = new ViewPortOptions
                {
                    Width = 800,
                    Height = 600
                }
            });
        
        await using var page = await browser.NewPageAsync();
        await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        
        await page.GoToAsync(tweetUrl);
        
        await page.WaitForSelectorAsync("article");
        
        var tweet = await page.QuerySelectorAsync("article"); // https://x.com/jack/status/20
        
        if (tweet == null)
        {
            await browser.CloseAsync();
            throw new Exception("No se encontró el tweet.");
        }

        // Toma captura solo del tweet
        var screenshot = await tweet.ScreenshotDataAsync(new ElementScreenshotOptions
        {
            Type = ScreenshotType.Png
        });

        return screenshot;
    }
}