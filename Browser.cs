using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Browse
{
    public class Browser
    {
        public IWebDriver Driver { get; set; }
        private ChromeOptions ChromeOptions { get; set; } = new ChromeOptions();
        private ChromeDriverService ChromeDriverService { get; set; } = ChromeDriverService.CreateDefaultService();
        public void DriverSettingsSetter()
        {
            ChromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions.AddArgument("--headless");
            Driver = new ChromeDriver(options: ChromeOptions, service: ChromeDriverService);
        }
        public ReadOnlyCollection<IWebElement> UpdateDialogsLI(string liKCssSelectorKey)
        {
            return Driver.FindElement(By.CssSelector(liKCssSelectorKey)).FindElements(By.XPath("li"));
        }
        public Browser()
        {
            DriverSettingsSetter();
        }
    }
}
