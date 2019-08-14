using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Configuration;
using System.Collections.Specialized;
using OpenQA.Selenium.Support.UI;

namespace QREST_BrowserUnitTests
{
	public static class BrowserProperties
	{
		public static IWebDriver SetTestDriver( string browserType, string browserVersion, string os, string osVersion, string resolution, string testName, string browserStackDebug)
		{
			IWebDriver driver;
			DesiredCapabilities capability = new DesiredCapabilities();
			capability.SetCapability("browser", browserType);
			capability.SetCapability("browser_version", browserVersion);
			capability.SetCapability("os", os);
			capability.SetCapability("os_version", osVersion);
			capability.SetCapability("resolution", resolution);
			capability.SetCapability("browserstack.user", ConfigurationManager.AppSettings["browserStackUser"]);
			capability.SetCapability("browserstack.key", ConfigurationManager.AppSettings["browserStackKey"]);
			capability.SetCapability("name", testName);
			capability.SetCapability("browserstack.debug", browserStackDebug);

			driver = new RemoteWebDriver(new Uri(ConfigurationManager.AppSettings["remoteWebDriverUrl"]), capability, TimeSpan.FromSeconds(180));
			return driver;
		}

		public static void SetFieldValues(NameValueCollection nameValueCollection, ref IWebDriver driver)
		{
			foreach(string key in nameValueCollection)
			{
				string tagname = driver.FindElement(By.Name(key)).TagName;
				if (tagname == "input")
				{
					driver.FindElement(By.Name(key)).SendKeys(Keys.Control + "a");
					driver.FindElement(By.Name(key)).SendKeys(nameValueCollection[key]);
				}
				else if (tagname == "select")
				{
					var select = driver.FindElement(By.Name(key));
					var selectElement = new SelectElement(select);
					selectElement.SelectByValue(nameValueCollection[key]);
				}
				
			}
		}
	}
}
