using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsScrapper.Models;
using OpenQA.Selenium.Support.UI;

namespace NewsScrapper.Services
{
   public class GoogleNewsScraperService
    {
        public async Task<List<NewsArticle>> ScrapeGoogleNewsAsync(string searchQuery)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36");
            
            using (var driver = new ChromeDriver(options))
            {
                var articles = new List<NewsArticle>();
                
                try
                {
                    driver.Navigate().GoToUrl($"https://news.google.com/search?q={Uri.EscapeDataString(searchQuery)}&hl=tr&gl=TR");
                    
                    await Task.Delay(5000);
                    
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    
                    // Log the page source for debugging
                   
                    Console.WriteLine("Page Source:");
                    Console.WriteLine(driver.PageSource);
                    
                    // Updated CSS selectors based on provided HTML
                    var articleElements = driver.FindElements(By.CssSelector("article.IFHyqb.DeXSAc"));

                    Console.WriteLine($"Found {articleElements.Count} article elements");
                    
                    foreach (var articleElement in articleElements.Take(50))
                    {
                        try
                        {
                            var newsArticle = new NewsArticle();

                            // Extract title and link
                            var titleElement = articleElement.FindElements(By.CssSelector("a.JtKRv")).FirstOrDefault();
                            if (titleElement != null)
                            {
                                newsArticle.Title = titleElement.Text;
                                newsArticle.Link = titleElement.GetAttribute("href");
                            }

                            // Extract image URL
                            var imageElement = articleElement.FindElements(By.CssSelector("img.Quavad.vwBmvb")).FirstOrDefault();
                            if (imageElement != null)
                            {
                                newsArticle.ImageUrl = imageElement.GetAttribute("src");
                            }

                            // Extract publish date
                            var timeElement = articleElement.FindElements(By.CssSelector("time.hvbAAd")).FirstOrDefault();
                            if (timeElement != null)
                            {
                                newsArticle.PublishDate = timeElement.GetAttribute("datetime") ?? timeElement.Text;
                            }

                            if (!string.IsNullOrEmpty(newsArticle.Title))
                            {
                                articles.Add(newsArticle);
                                Console.WriteLine($"Added article: {newsArticle.Title}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error extracting article: {ex.Message}");
                        }
                    }
                    
                    if (articles.Count == 0)
                    {
                        Console.WriteLine("No articles found. Trying alternative approach...");
                        articles = await ScrapeGoogleSearchAsync(driver, searchQuery);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during scraping: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
                
                return articles;
            }
        }
        
        private async Task<List<NewsArticle>> ScrapeGoogleSearchAsync(IWebDriver driver, string searchQuery)
        {
            var articles = new List<NewsArticle>();
            
            try
            {
                driver.Navigate().GoToUrl($"https://www.google.com/search?q={Uri.EscapeDataString(searchQuery)}&tbm=nws&hl=tr");
                await Task.Delay(3000);
                
                var newsResults = driver.FindElements(By.CssSelector("div.SoaBEf, div.WlydOe"));
                
                foreach (var result in newsResults.Take(50))
                {
                    try
                    {
                        var newsArticle = new NewsArticle();
                        
                        var titleElement = result.FindElements(By.CssSelector("h3, div.mCBkyc")).FirstOrDefault();
                        newsArticle.Title = titleElement?.Text ?? string.Empty;
                        
                        var linkElement = result.FindElements(By.CssSelector("a")).FirstOrDefault();
                        newsArticle.Link = linkElement?.GetAttribute("href") ?? string.Empty;
                        
                        var imageElement = result.FindElements(By.CssSelector("img")).FirstOrDefault();
                        newsArticle.ImageUrl = imageElement?.GetAttribute("src") ?? string.Empty;
                        
                        var timeElement = result.FindElements(By.CssSelector("span.OSrXXb")).FirstOrDefault();
                        newsArticle.PublishDate = timeElement?.Text ?? string.Empty;
                        
                        if (!string.IsNullOrEmpty(newsArticle.Title))
                        {
                            articles.Add(newsArticle);
                            Console.WriteLine($"Added article from search: {newsArticle.Title}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error extracting search result: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Google Search scraping: {ex.Message}");
            }
            
            return articles;
        }
    }
}

