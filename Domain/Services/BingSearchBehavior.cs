using Domain.Services.Interface;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Playwright;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;

namespace Domain.Services
{
    public class BingSearchBehavior : ISearchBehavior
    {
        private readonly IMemoryCache _memoryCache;

        public BingSearchBehavior(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<int> CheckRank(string keyword, string urlToFind)
        {

            int? result;
            string cacheKey = $"bing-{keyword}-{urlToFind}";
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
                    Args = new[] { "--disable-webrtc", "--disable-blink-features=AutomationControlled" }
                });
                var page = await browser.NewPageAsync();
                var maximumRecords = Constant.MaximumRecords; // Set the maximum number of records 
                Random random = new Random();
                int randomNumber = random.Next(1000, 5000);

                await page.WaitForTimeoutAsync(randomNumber);
                var index = -1;
                var first = 1;
                while (index == -1 && first < maximumRecords)
                {
                    // Navigate to Bing with keyword and first record of each page
                    await page.GotoAsync($"https://www.bing.com.au/search?q={keyword}&first={first}");

                    await page.WaitForTimeoutAsync(randomNumber);
                    // Extract search results
                    var links = (await page.Locator("a.tilk").EvaluateAllAsync<string[]>("elements => elements.map(element => element.href)")).ToList();
                    index = links.FindIndex(link => link.Contains(urlToFind));

                    if (index == -1) first = first + 10;
                }
                // Close the browser
                await browser.CloseAsync();
                result = index == -1 ? 0 : index + first;
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
