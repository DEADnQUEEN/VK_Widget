using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Browse
{
    public class Browser
    {
        public IWebDriver Driver { get; set; }
        private EdgeOptions EdgeOptions { get; set; } = new EdgeOptions();
        private EdgeDriverService EdgeDriverService { get; set; } = EdgeDriverService.CreateDefaultService();
        public void DriverSettingsSetter()
        {
            EdgeOptions.AddArgument("--headless");
            EdgeOptions.AddArgument("--mute-audio");
            Driver = new EdgeDriver(options: EdgeOptions, service: EdgeDriverService);
        }
        public Browser()
        {
            EdgeDriverService.HideCommandPromptWindow = true;
            DriverSettingsSetter();
        }
    }
}
