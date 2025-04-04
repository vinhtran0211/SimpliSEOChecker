using Domain.Services.Interface;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Playwright;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Services
{
    public class GoogleSearchBehavior : ISearchBehavior
    {
        private readonly IMemoryCache _memoryCache;

        public GoogleSearchBehavior(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<int> CheckRank(string keyword, string urlToFind)
        {
            int? result;
            string cacheKey = $"google-{keyword}-{urlToFind}";
            result = _memoryCache.Get<int?>(cacheKey);
            if (result != default)
            {
                return result.GetValueOrDefault();
            }

            try
            {
                // Initialize Playwright
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless= false,
                    Args = new[] { "--disable-webrtc", "--disable-blink-features=AutomationControlled" }
                });
                var page = await browser.NewPageAsync();
                var maximumRecords = Constant.MaximumRecords; // Set the maximum number of records 

                Random random = new Random();
                int randomNumber = random.Next(1000, 5000);

                await page.WaitForTimeoutAsync(randomNumber);

                // Navigate to Google with keyword and maximum number of records 
                await page.GotoAsync($"https://www.google.com.au/search?q={keyword}&num={maximumRecords}");

                await page.WaitForTimeoutAsync(randomNumber);
                // Extract search results
                var links = await page.Locator("a.zReHs").EvaluateAllAsync<string[]>("elements => elements.map(element => element.href)");
                int index = links.ToList().FindIndex(link => link.Contains(urlToFind));
                // Close the browser
                await browser.CloseAsync();
                result = index + 1;

                _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(Constant.CacheTimeInMinutes)); // Cache the result for 60 minutes
                return result.GetValueOrDefault();
            }
            catch
            {
                return -1;
            }

        }
    }
}
