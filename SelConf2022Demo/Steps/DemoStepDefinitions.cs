using NFluent;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V103.Emulation;
using System;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;

namespace SelConf2022Demo.Steps
{
    [Binding]
    public class DemoStepDefinitions
    {


        private readonly ScenarioContext _scenarioContext;

        protected IDevTools devTools;
        protected IDevToolsSession session;

        protected IWebDriver driver;
        protected DevToolsSessionDomains devToolsSession;
        private readonly string _driverTargetPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public DemoStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void Initialize()
        {
            OpenBrowser();
        }

        private void OpenBrowser()
        {
            driver = new ChromeDriver(_driverTargetPath, SetupChromeOpts());
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
        }

        private ChromeOptions SetupChromeOpts()
        {
            var options = new ChromeOptions();

            options.AddArguments("--start-maximized", "--disable-notifications");
            return options;
        }


        [AfterScenario]
        public void TestStop()
        {
            driver.Dispose();
        }

        [Given("i navigate to (.*) site")]
        public void NavigateToURL(string site)
        {
            switch (site.ToLowerInvariant())
            {
                case "google":
                    driver.Navigate().GoToUrl("https://www.google.com");
                    break;
                case "duckduckgo":
                    driver.Navigate().GoToUrl("https://duckduckgo.com/");
                    break;
                default:
                    throw new ArgumentException($"{site} is not available");
            }

        }

        [When(@"I change the browser settings to (.*) emulating a (.*)")]
        [Then(@"I change the browser settings to (.*) emulating a (.*)")]
        public void WhenIChangeTheBrowserSettingsToStartEmulatingAIPHONEPRO(string emulatorstate, string mobName)
        {
            switch (emulatorstate.ToLowerInvariant())
            {
                case "start":
                    //driver.SwitchTo().NewWindow(WindowType.Tab);

                    devTools = driver as IDevTools;
                    //DevTools Session
                    session = devTools.GetDevToolsSession();

                    session
                        .GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V103.DevToolsSessionDomains>()
                        .Emulation
                        .SetDeviceMetricsOverride(GetDeviceModeSettings(mobName));
                    break;
                case "end":
                    session
                        .GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V103.DevToolsSessionDomains>()
                        .Emulation
                        .ClearDeviceMetricsOverride();
                    session.Dispose();
                    break;
                default:
                    throw new ArgumentException($"{emulatorstate} is not available");

            }
        }


        [Given("i get the page title for (.*)")]
        [When("i get the page title for (.*)")]
        public void GivenTheSecondNumberIs(string site)
        {
            _scenarioContext[$"pagetitle_{site}"] =driver.Title;
            Check.That(driver.PageSource.Contains("Gmail")).IsTrue();
        }

        [Then("i validate the page titles")]
        public void ThenTheResultShouldBe()
        {
            Check.That(_scenarioContext.Get<string>("pagetitle_webbrowser")).IsEqualTo(_scenarioContext.Get<string>("pagetitle_emulator"));
        }

        public SetDeviceMetricsOverrideCommandSettings GetDeviceModeSettings(string mobName) => (mobName.ToUpperInvariant()) switch
        {

            "IPHONE11PRO" => new SetDeviceMetricsOverrideCommandSettings
            {
                Width = 375,
                Height = 812,
                Mobile = true,
                DeviceScaleFactor = 50
            },
            "SAMSUNGS20" => new SetDeviceMetricsOverrideCommandSettings
            {
                Width = 360,
                Height = 800,
                Mobile = true,
                DeviceScaleFactor = 50
            },
            _ => throw new ArgumentException($"{mobName} undefined"),
        };
    }
}
