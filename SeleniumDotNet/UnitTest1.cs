using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace SeleniumDotNet
{
    public class UnitTest1 : IDisposable
    {
        private readonly WebDriverFactory webDriverFactory;
        private readonly WineFactory wineFactory;
        private readonly ChromeDriver driver;

        private readonly string rootUrl =
            "https://www.winemag.com";

        private readonly string driverPath;
        private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
        private readonly TestDbContext dbContext;

        public UnitTest1()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());

            var config = new ConfigurationBuilder()
                .AddJsonFile("test.settings.json", optional: false)
                .Build();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(config["ConnectionString"])
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(this.loggerFactory);

            this.dbContext = new TestDbContext(dbContextOptionsBuilder.Options);
            this.webDriverFactory = new WebDriverFactory();
            this.wineFactory = new WineFactory(TimeSpan.FromSeconds(3));
            
        private readonly ChromeDriver driver;

        private readonly string rootUrl =
            "https://www.vivino.com";

        private readonly string driverPath;

        public UnitTest1()
        {
            this.webDriverFactory = new WebDriverFactory();
            this.driverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "../../../", "chromedriver");
            this.driver = this.webDriverFactory.Create(this.driverPath, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task Test1()
        {
            var currentPage = 1;
            var totalPages = Int32.MaxValue;
            var reviewsPerPage = 6;
            var wineType = "Red";
            var reviewYear = 2021;
            var country = "US";
            var js = (IJavaScriptExecutor)this.driver;

            while (currentPage <= totalPages)
            {
                this.driver.Navigate().GoToUrl($"{this.rootUrl}/wine-ratings/?s=&drink_type=wine&wine_type={wineType}&country={country}&pub_date_web={reviewYear}&page={currentPage}&sort_by=brand&sort_dir=asc");
                await Task.Delay(TimeSpan.FromSeconds(3));
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(TimeSpan.FromSeconds(3));

                if (currentPage > 1)
                {
                    var resultsElement = this.driver.FindElementByCssSelector("[class='results']");
                    var totalReviews = int.Parse(resultsElement.GetAttribute("data-total-reviews"));

                    // set the total pages
                    totalPages = (int)Math.Ceiling(totalReviews / (decimal)reviewsPerPage);
                }

                var reviewListingElements = this.driver.FindElementsByCssSelector("[class*='review-listing']");

                foreach (var reviewListingElement in reviewListingElements)
                {
                    var reviewUrl = reviewListingElement.GetAttribute("href");
                    var dto = new ReviewUrlDto
                    {
                        Id = HashUrl(reviewUrl),
                        Value = reviewUrl,
                    };

                    try
                    {
                        await this.dbContext.ReviewUrls.AddAsync(dto);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    
                }

                await this.dbContext.SaveChangesAsync();
                currentPage++;

            }
        }

        private string HashUrl(string reviewUrl)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            using(md5)
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(reviewUrl);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Step 2, convert byte array to hex string
                var sb = new StringBuilder();
                
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        [Theory]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWKBkEZA0MlErti1PBAB5pxe1")]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWFsjtSIQaaJWbFueCAB5tBe2")]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWFtjtSIgaWSiVmxbnggAecEXtw%3D%3D")]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWFsTtSIgaWSiVmxbnggAec4XuA%3D%3D")]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWFtztSIgaWSiVmxbnggAefUXuw%3D%3D")]
        [InlineData("https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWFsjE7UiCFVsW54IAJBtF-o%3D")]
        public async Task ScrollTest(string url)
        {
            this.driver.Navigate().GoToUrl(url);
            await Task.Delay(TimeSpan.FromSeconds(3));

            var js = (IJavaScriptExecutor)this.driver;
            var lastHeight = (long)js.ExecuteScript("return document.body.scrollHeight");

            while (true)
            {
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                await Task.Delay(TimeSpan.FromSeconds(5));

                var currentHeight = (long)js.ExecuteScript("return document.body.scrollHeight");
                if (currentHeight == lastHeight)
                {
                    break;
                }

                lastHeight = currentHeight;
            }

            var anchorElements = this.driver.FindElementsByCssSelector("[class*='wineCard__cardLink']");
            var hrefSet = new HashSet<string>();

            foreach (var anchorElement in anchorElements)
            {
                var href = anchorElement.GetAttribute("href");
                hrefSet.Add(href.ToLowerInvariant());
            }

            foreach (var href in hrefSet)
            {
                await this.dbContext.ReviewUrls.AddAsync(new ReviewUrlDto
                {
                    Id = HashUrl(href),
                    Value = href,
                });

                await this.dbContext.SaveChangesAsync();
            }

            this.driver.Manage().Window.Maximize();
            this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            this.driver.Navigate().GoToUrl($"{this.rootUrl}/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWKBksW15IgDbDRXC");
            await Task.Delay(TimeSpan.FromSeconds(3));

            var anchorElements = this.driver.FindElementsByCssSelector("[class*='wineCard__cardLink']");
            var wines = new List<Wine>();

            foreach (var anchorElement in anchorElements)
            {
                var subDriver = this.webDriverFactory.Create(this.driverPath, TimeSpan.FromSeconds(3));
                subDriver.Manage().Window.Maximize();
                subDriver.Navigate().GoToUrl(anchorElement.GetAttribute("href"));

                var wine = new Wine();

                wine.Name = subDriver.FindElementByCssSelector("[class='wine']").Text;
                wine.Winery = subDriver.FindElementByCssSelector("[class='winery']").Text;
                wine.Vintage = subDriver.FindElementByCssSelector("[class='vintage']").Text;
                wine.Price = subDriver.FindElementByCssSelector("[class*= 'purchaseAvailability']").Text;

                var js = (IJavaScriptExecutor) subDriver;
                js.ExecuteScript("window.scrollBy(0, window.innerHeight)");
                await Task.Delay(TimeSpan.FromSeconds(3));


                var tastingNoteElements =
                    subDriver.FindElementsByCssSelector("[class*='tasteNote__popularKeywords']");

                foreach (var tastingNoteElement in tastingNoteElements)
                {
                    var words = tastingNoteElement.Text.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var word in words)
                    {
                        wine.TastingNotes.Add(word);
                    }
                }

                wines.Add(wine);
                subDriver.Quit();
                await Task.Delay(TimeSpan.FromSeconds(3));

            }
            
            wines.Count.Should().Be(25);

        }

        public void Dispose()
        {
            this.driver.Quit();
            this.dbContext.Dispose();
            this.driver?.Quit();
        }
    }
}
