using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace SeleniumDotNet
{
    public class WebDriverFactory
    {
        public ChromeDriver Create(
            string driverPath,
            TimeSpan timeout)
        {
            var driver = new ChromeDriver(driverPath);
            driver.Manage().Timeouts().ImplicitWait = timeout;

            return driver;
        }
    }
}
