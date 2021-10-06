using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumDotNet
{
    public class WineFactory
    {
        private readonly TimeSpan delay;

        public WineFactory(TimeSpan delay)
        {
            this.delay = delay;
        }

        public async Task<Wine> CreateAsync(ChromeDriver subDriver)
        {
            var wine = new Wine
            {
                Name = subDriver.FindElementByCssSelector("[class='wine']").Text,
                Winery = subDriver.FindElementByCssSelector("[class='winery']").Text,
                Vintage = subDriver.FindElementByCssSelector("[class='vintage']").Text,
                Price = subDriver.FindElementByCssSelector("[class*= 'purchaseAvailability']").Text
            };


            var js = (IJavaScriptExecutor)subDriver;
            js.ExecuteScript("window.scrollBy(0, window.innerHeight)");
            await Task.Delay(this.delay);


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

            return wine;
        }
    }
}