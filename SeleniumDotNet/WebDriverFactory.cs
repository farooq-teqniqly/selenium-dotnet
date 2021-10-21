using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace SeleniumDotNet
{
    public class WebDriverFactory
    {
        public ChromeDriver Create(
            string driverPath,
            TimeSpan timeout)
        {
            var options = new ChromeOptions();
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.109 Safari/537.36");
            var driver = new ChromeDriver(driverPath);
            driver.Manage().Timeouts().ImplicitWait = timeout;
            driver.Manage().Window.Maximize();

          var driver = new ChromeDriver(driverPath);
            driver.Manage().Timeouts().ImplicitWait = timeout;

            return driver;
        }
    }
}
