using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace QREST_BrowserUnitTests
{
	[TestClass]
	public class SeleniumBrowserStackTests
	{
		string decryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		/*driver.FindElement(By.Id("Username")).SendKeys(QRestUserName);
			driver.FindElement(By.Id("Password")).SendKeys(ConfigurationManager.AppSettings["QRestPassword"]);*/
		string QRestUserName = "";
		string QRestPassword = "";
		string browserStackUser = "";
		string browserStackKey = "";

		public SeleniumBrowserStackTests()
		{
			QRestUserName = Decrypt(ConfigurationManager.AppSettings["QRestUserName"]);
			QRestPassword = Decrypt(ConfigurationManager.AppSettings["QRestPassword"]);
			browserStackUser = Decrypt(ConfigurationManager.AppSettings["browserStackUser"]);
			browserStackKey = Decrypt(ConfigurationManager.AppSettings["browserStackKey"]);
		}
		string siteEditId = "a02aef17-09c3-4e60-bdb0-2b6db3bc886a";
		string siteDeleteId = "a02aef17-09c3-4e60-bdb0-2b6db3bc886a";
		string siteMonitorEditId = "0296bca0-f2ac-4d63-a466-bc6e09c15a70";
		string siteMonitorDeleteId = "0296bca0-f2ac-4d63-a466-bc6e09c15a70";

		#region Chrome
		[TestMethod]
		public void ChromeSiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Chrome");
			driver.Quit();
		}

		[TestMethod]
		public void ChromeSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
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
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Firefox");
			driver.Quit();
		}

		[TestMethod]
		public void FireFoxSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Firefox");
			driver.Quit();
		}
		#endregion

		#region IE

		[TestMethod]
		public void IESiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "IE");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "IE");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "IE");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "IE");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "IE");
			driver.Quit();
		}

		[TestMethod]
		public void IESiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "IE");
			driver.Quit();
		}
		#endregion

		#region Edge

		[TestMethod]
		public void EdgeSiteListTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteTest(driver, "Edge");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteTest(driver, "Edge");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteTest(driver, "Edge");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorDeleteTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			deleteSiteMonitorTest(driver, "Edge");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorAddTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			addSiteMonitorTest(driver, "Edge");
			driver.Quit();
		}

		[TestMethod]
		public void EdgeSiteMonitorEditTest()
		{
			string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
			IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
			loginToQRest(loginUrl, driver);
			editSiteMonitorTest(driver, "Edge");
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

		private void loginToQRest(string loginUrl, IWebDriver driver)
		{
			driver.Navigate().GoToUrl(loginUrl);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
			wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
			driver.FindElement(By.Id("Username")).SendKeys(QRestUserName);
			driver.FindElement(By.Id("Password")).SendKeys(QRestPassword);
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

		private string Decrypt(string cipherText)
		{
			string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			cipherText = cipherText.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
			0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
		});
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return cipherText;
		}
	}
}
