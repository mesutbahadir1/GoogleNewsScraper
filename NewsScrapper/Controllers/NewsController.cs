using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsScrapper.Models;
using NewsScrapper.Services;

namespace NewsScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly GoogleNewsScraperService _scraperService;
        
        public NewsController(GoogleNewsScraperService scraperService)
        {
            _scraperService = scraperService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetNews([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query is required");
            }
            
            try
            {
                var articles = await _scraperService.ScrapeGoogleNewsAsync(query);
                
                if (articles.Count == 0)
                {
                    return NotFound("No news articles found for the given query. This could be due to no results or issues with scraping.");
                }
                
                return Ok(articles);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred while scraping news: {ex.Message}. Please check the server logs for more details.");
            }
        }
    }
}
