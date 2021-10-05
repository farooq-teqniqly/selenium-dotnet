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
        private readonly ChromeDriver driver;

        private readonly string rootUrl =
            "https://www.vivino.com/explore?e=eJzLLbI1VMvNzLM1UMtNrLA1NTBQS660DQ1WSwYSLmoFQNn0NNuyxKLM1JLEHLX8ohRbtfykSlu18pLoWKBksW15IgDbDRXC";

        public UnitTest1()
        {
            var driverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "../../../", "chromedriver");
            this.driver = new ChromeDriver(driverPath);
        }

        [Fact]
        public async Task Test1()
        {
            this.driver.Manage().Window.Maximize();
            this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            this.driver.Navigate().GoToUrl(this.rootUrl);
            await Task.Delay(TimeSpan.FromSeconds(3));

            var wineCardElements = this.driver.FindElementsByCssSelector("[class*= 'wineCard__wineCardContent']");
            var wines = new List<Wine>();

            foreach (var wineCardElement in wineCardElements)
            {
                wineCardElement.Click();
                await Task.Delay(TimeSpan.FromSeconds(3));

                var wine = new Wine();

                wine.Name = this.driver.FindElementByCssSelector("[class='wine']").Text;
                wine.Winery = this.driver.FindElementByCssSelector("[class='winery']").Text;
                wine.Vintage = this.driver.FindElementByCssSelector("[class='vintage']").Text;
                wine.Price = this.driver.FindElementByCssSelector("[class*= 'purchaseAvailability']").Text;

                var tastingNoteElements =
                    this.driver.FindElementsByCssSelector("[class*= 'tasteNote__popularKeywords']");

                foreach (var tastingNoteElement in tastingNoteElements)
                {
                    var words = tastingNoteElement.Text.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var word in words)
                    {
                        wine.TastingNotes.Add(word);
                    }
                }

                wines.Add(wine);
                this.driver.Navigate().Back();
                await Task.Delay(TimeSpan.FromSeconds(3));
            }

            wines.Count.Should().Be(25);

        }

        public void Dispose()
        {
            this.driver?.Dispose();
        }
    }
}
