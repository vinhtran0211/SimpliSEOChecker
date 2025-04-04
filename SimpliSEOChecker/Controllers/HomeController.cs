using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SimpliSEOChecker.Models;

namespace SimpliSEOChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetRank(HomeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input");
            }
            var search = new SearchStrategy(new GoogleSearchBehavior(_memoryCache));

            Task<int> googleRank = search.DoCheckRank(model.Keyword, model.UrlToFind); //Sometimes, Google will display a CAPTCHA page, which prevents results from being found

            search.SetBehavior(new BingSearchBehavior(_memoryCache));
            Task<int> bingRank = search.DoCheckRank(model.Keyword, model.UrlToFind);
            // Wait for all tasks to complete
            await Task.WhenAll(googleRank, bingRank);

            ViewBag.IsSubmitted = true;
            ViewBag.GoogleRank = googleRank.Result;
            ViewBag.BingRank = bingRank.Result;
            ViewBag.UrlToFind = model.UrlToFind;
            ViewBag.Keyword = model.Keyword;

            return View("Index");
        }
    }
}
