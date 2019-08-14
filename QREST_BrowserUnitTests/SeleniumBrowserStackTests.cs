using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace QREST_BrowserUnitTests
{
	[TestClass]
	public class SeleniumBrowserStackTests
	{
		string siteEditId = "6f1528c7-8106-48c9-83d6-8e60ac2b56a3";
		string siteDeleteId = "ae1860da-3400-4b41-8f7a-dded8db5fc3c";
		string siteMonitorEditId = "2cf555dc-6442-4f13-b4c1-6021bd73bebf";
		string siteMonitorDeleteId = "2cf555dc-6442-4f13-b4c1-6021bd73bebf";

		#region Chrome
		[TestMethod]
		public void ChromeSiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}
		#endregion

		#region FireFox

		[TestMethod]
		public void FireFoxSiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}
		#endregion

		#region IE

		[TestMethod]
		public void IESiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}
		#endregion

		#region Edge

		[TestMethod]
		public void EdgeSiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true");
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true");
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true");
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}
		#endregion

		private void deleteSiteTest(IWebDriver driver, string browser)
		{
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			var deleteSections = driver.FindElement(By.CssSelector($"[data-delete-id*='{siteDeleteId}'"));
			deleteSections.FindElement(By.XPath("..")).FindElement(By.ClassName("delete-link")).Click();
			deleteSections.Click();
			Console.WriteLine(driver.Title);
			driver.Quit();
		}

		private void deleteSiteMonitorTest(IWebDriver driver, string browser)
		{
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
			var deleteSections = driver.FindElement(By.CssSelector($"[data-delete-id*='{siteMonitorDeleteId}'"));
			deleteSections.FindElement(By.XPath("..")).FindElement(By.ClassName("delete-link")).Click();
			deleteSections.Click();
			Console.WriteLine(driver.Title);
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Firefox", "true");
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Firefox", "true");
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Firefox");
			driver.Quit();
		}


		private void loginToQRest(string loginUrl, IWebDriver driver)
		{
			driver.Navigate().GoToUrl(loginUrl);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.Id("Username")).SendKeys(ConfigurationManager.AppSettings["QRestUserName"]);
			driver.FindElement(By.Id("Password")).SendKeys(ConfigurationManager.AppSettings["QRestPassword"]);
			driver.FindElements(By.ClassName("btn")).Where(x => x.Text.ToLower() == "sign in").FirstOrDefault().Click();
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			Console.WriteLine(driver.Title);
		}

		private void addSiteTest(IWebDriver driver, string browser)
		{
			NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
			siteAddEditFieldCollection["SITE_NAME"] = $"{browser}_Add";
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.LinkText("Add")).Click();
			BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
			driver.FindElement(By.Id("btnSave")).Click();
			Console.WriteLine(driver.Title);

		}

		private void editSiteTest(IWebDriver driver, string browser)
		{
			NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
			siteAddEditFieldCollection["SITE_NAME"] = $"{browser}_Edit";
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
			BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
			driver.FindElement(By.Id("btnSave")).Click();
			Console.WriteLine(driver.Title);
		}

		private void addSiteMonitorTest(IWebDriver driver, string browser)
		{
			NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/MonitorEdit?siteIDX={siteEditId}'")).Click();
			driver.FindElement(By.Id("add-it")).Click();
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector("a[data-id='9789f503-d325-4cab-8fa7-c1cc4bfc0659']")).Click();
			BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
			driver.FindElement(By.Id("btnSave")).Click();
			driver.FindElement(By.LinkText("Back to List")).Click();
			Console.WriteLine(driver.Title);
		}

		private void editSiteMonitorTest(IWebDriver driver, string browser)
		{
			NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
			wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.CssSelector($"[href*='/Site/MonitorEdit/{siteMonitorEditId}']")).Click();
			BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
			driver.FindElement(By.Id("btnSave")).Click();
			driver.FindElement(By.LinkText("Back to List")).Click();
			Console.WriteLine(driver.Title);
		}

		private NameValueCollection getSiteAddFieldCollection()
		{
			NameValueCollection siteAddEditFieldCollection = new NameValueCollection();
			siteAddEditFieldCollection.Add("SITE_ID", "Test_Chrome");
			siteAddEditFieldCollection.Add("SITE_NAME", "Chrome Add");
			siteAddEditFieldCollection.Add("ORG_ID", "TEST");
			siteAddEditFieldCollection.Add("AQS_SITE_ID", "1");
			siteAddEditFieldCollection.Add("SITE_COMMENTS", "Test through BrowserStack for Chrome");
			siteAddEditFieldCollection.Add("LATITUDE", "36.19730");
			siteAddEditFieldCollection.Add("LONGITUDE", "-95.87295");
			siteAddEditFieldCollection.Add("ADDRESS", "Test Address");
			siteAddEditFieldCollection.Add("CITY", "Test City");
			siteAddEditFieldCollection.Add("STATE", "NJ");
			siteAddEditFieldCollection.Add("ZIP_CODE", "12345");
			return siteAddEditFieldCollection;
		}

		private NameValueCollection getSiteMonitorAddFieldCollection()
		{
			NameValueCollection siteMonitorAddEditFieldCollection = new NameValueCollection();
			siteMonitorAddEditFieldCollection.Add("POC", "102");
			siteMonitorAddEditFieldCollection.Add("DURATION_CODE", "1");
			siteMonitorAddEditFieldCollection.Add("COLLECT_FREQ_CODE", "B");
			siteMonitorAddEditFieldCollection.Add("ALERT_MIN_VALUE", "5");
			siteMonitorAddEditFieldCollection.Add("ALERT_MAX_VALUE", "10");
			siteMonitorAddEditFieldCollection.Add("ALERT_PCT_CHANGE", "6");
			siteMonitorAddEditFieldCollection.Add("ALERT_STUCK_REC_COUNT", "6");
			return siteMonitorAddEditFieldCollection;
		}
	}
}
