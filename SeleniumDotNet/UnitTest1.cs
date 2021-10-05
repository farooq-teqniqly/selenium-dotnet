using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            this.driver?.Quit();
        }
    }
}
