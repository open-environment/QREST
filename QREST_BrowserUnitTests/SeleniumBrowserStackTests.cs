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

        //Todo: Need to remove commented code once reviewed

        //string decryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //driver.FindElement(By.Id("Username")).SendKeys(QRestUserName);
		//driver.FindElement(By.Id("Password")).SendKeys(ConfigurationManager.AppSettings["QRestPassword"]);
        string QRestUserName = "";
        string QRestPassword = "";
        string browserStackUser = "";
        string browserStackKey = "";

        public SeleniumBrowserStackTests()
        {
            //QRestUserName = Decrypt(ConfigurationManager.AppSettings["QRestUserName"]);
            //QRestPassword = Decrypt(ConfigurationManager.AppSettings["QRestPassword"]);
            QRestUserName = "kshitij.mehta@open-environment.org";
            QRestPassword = "Mehta@2020";

            browserStackUser = Decrypt(ConfigurationManager.AppSettings["browserStackUser"]);
            browserStackKey = Decrypt(ConfigurationManager.AppSettings["browserStackKey"]);
        }

        //string siteEditId = "a02aef17-09c3-4e60-bdb0-2b6db3bc886a";
        //string siteDeleteId = "a02aef17-09c3-4e60-bdb0-2b6db3bc886a";
        //string siteMonitorEditId = "0296bca0-f2ac-4d63-a466-bc6e09c15a70";
        //string siteMonitorDeleteId = "0296bca0-f2ac-4d63-a466-bc6e09c15a70";

        #region Chrome

        [TestMethod]
        public void ChromeSiteListTest()
        {
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site List, Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteAddTest()
        {
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site Add Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            addSiteTest(driver, "Chrome");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteEditTest()
        {

            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site Edit Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            editSiteTest(driver, "Chrome");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteDeleteTest()
        {
            string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site Delete Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            deleteSiteTest(driver, "Chrome");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteMonitorDeleteTest()
        {
            string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site Delete Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            deleteSiteMonitorTest(driver, "Chrome");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteMonitorAddTest()
        {
            string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Add Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            addSiteMonitorTest(driver, "Chrome");
            driver.Quit();
        }

        [TestMethod]
        public void ChromeSiteMonitorEditTest()
        {
            string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QRest Site Edit Chrome", "true", browserStackUser, browserStackKey);
            loginToQREST(driver);
            editSiteMonitorTest(driver, "Chrome");
            driver.Quit();
        }
        #endregion

        private void loginToQREST(IWebDriver driver)
        {
            string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
            driver.Navigate().GoToUrl(loginUrl);
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            //wait for login page to load
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
            string returnVal = string.Empty;
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            driver.FindElement(By.LinkText("Add")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
            BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
            driver.FindElement(By.Id("btnSave")).Click();

            //verify it has been added
            //driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //var deleteSections = driver.FindElement(By.CssSelector($"[data-delete-id*='{siteDeleteId}'"));

            //Try to get the newly created SiteID and pass it accross test chain
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //string newSiteID = driver.Url.Substring(driver.Url.LastIndexOf("/") + 1);
            //if (newSiteID.Length > 0 && newSiteID.Contains("SiteEdit") == false)
            //{
            //    //Verify it has been added
            //    NameValueCollection siteAddEditFieldCollectionProcessed = getSiteAddFieldCollectionProcessed(driver);
            //    if (!IsCollectionMatch(siteAddEditFieldCollection, siteAddEditFieldCollectionProcessed))
            //    {
            //        Assert.Fail("addSiteTest: Data values does not match");
            //    }
            //    returnVal = newSiteID;

            //}
            //else
            //{
            //    Assert.Fail("addSiteTest: unable to get new site id!");
            //}



            //return returnVal;

        }

        private void editSiteTest(IWebDriver driver, string browser)
        {
            NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
            siteAddEditFieldCollection["SITE_NAME"] = $"{browser}_Edit";
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
            driver.FindElement(By.XPath("//table[@id=\'gridData\']/tbody/tr/td/a")).Click();
            //By.XPath("//table[@id=\'gridData\']/tbody/tr/td/a")
            BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
            driver.FindElement(By.Id("btnSave")).Click();
            Console.WriteLine(driver.Title);
        }

        private void deleteSiteTest(IWebDriver driver, string browser)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //var deleteSections = driver.FindElement(By.CssSelector($"[data-delete-id*='{siteDeleteId}'"));
            //deleteSections.FindElement(By.XPath("..")).FindElement(By.ClassName("delete-link")).Click();
            //deleteSections.Click();
            driver.FindElement(By.XPath("//table[@id='gridData']/tbody/tr/td/div/a")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.XPath("//table[@id='gridData']/tbody/tr/td/div/div/b")).Click();
            driver.Quit();
        }

        private void deleteSiteMonitorTest(IWebDriver driver, string browser)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

            //driver.Navigate().GoToUrl("https://qrest-test.azurewebsites.net/Site/SiteList");
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            driver.Manage().Window.Size = new System.Drawing.Size(1056, 644);
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.CssSelector(".fa-times")).Click();
            driver.FindElement(By.CssSelector(".btn > b")).Click();
            driver.FindElement(By.LinkText("Back to List")).Click();

            //driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteEdit/{siteEditId}");

            //driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            //wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
            //var deleteSections = driver.FindElement(By.CssSelector($"[data-delete-id*='{siteMonitorDeleteId}'"));
            //deleteSections.FindElement(By.XPath("..")).FindElement(By.ClassName("delete-link")).Click();
            //deleteSections.Click();
            //Console.WriteLine(driver.Title);
            //driver.Quit();
        }

        private void addSiteMonitorTest(IWebDriver driver, string browser)
        {
            NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

            //driver.Navigate().GoToUrl("https://qrest-test.azurewebsites.net/Site/SiteList");
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            driver.Manage().Window.Size = new System.Drawing.Size(1052, 640);
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.LinkText("Add")).Click();
            driver.FindElement(By.Id("add-it")).Click();
            driver.FindElement(By.CssSelector("label > input")).SendKeys("11101");
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@data-id='54e82cab-26af-4a05-b4d8-8c44e3852da4']")));
            element.Click();
            //driver.FindElement(By.XPath(“//*[@gl-command='transaction']”)).Click();
            //driver.FindElement(By.LinkText("Select")).Click();
            driver.FindElement(By.Id("COLLECT_UNIT_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("COLLECT_UNIT_CODE"));
                dropdown.FindElement(By.XPath("//option[. = 'Micrograms']")).Click();
            }
            driver.FindElement(By.Id("COLLECT_UNIT_CODE")).Click();
            driver.FindElement(By.Id("POC")).Click();
            driver.FindElement(By.Id("POC")).SendKeys("111");
            driver.FindElement(By.Id("DURATION_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("DURATION_CODE"));
                dropdown.FindElement(By.XPath("//option[. = '1 WEEK']")).Click();
            }
            driver.FindElement(By.Id("DURATION_CODE")).Click();
            driver.FindElement(By.Id("COLLECT_FREQ_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("COLLECT_FREQ_CODE"));
                dropdown.FindElement(By.XPath("//option[. = 'RANDOM']")).Click();
            }
            driver.FindElement(By.Id("COLLECT_FREQ_CODE")).Click();
            driver.FindElement(By.Id("btnSave")).Click();
            //driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            //wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/MonitorEdit?siteIDX={siteEditId}'")).Click();
            //driver.FindElement(By.Id("add-it")).Click();
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector("a[data-id='9789f503-d325-4cab-8fa7-c1cc4bfc0659']")).Click();
            //BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
            //driver.FindElement(By.Id("btnSave")).Click();
            //driver.FindElement(By.LinkText("Back to List")).Click();
            //Console.WriteLine(driver.Title);
        }

        private void editSiteMonitorTest(IWebDriver driver, string browser)
        {
            NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

            //driver.Navigate().GoToUrl("https://qrest-test.azurewebsites.net/Site/SiteList");
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            driver.Manage().Window.Size = new System.Drawing.Size(1056, 644);
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Id("COLLECT_UNIT_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("COLLECT_UNIT_CODE"));
                dropdown.FindElement(By.XPath("//option[. = 'Microns']")).Click();
            }
            driver.FindElement(By.Id("COLLECT_UNIT_CODE")).Click();
            driver.FindElement(By.Id("POC")).Click();
            driver.FindElement(By.Id("POC")).SendKeys("222");
            driver.FindElement(By.Id("DURATION_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("DURATION_CODE"));
                dropdown.FindElement(By.XPath("//option[. = 'YEARLY']")).Click();
            }
            driver.FindElement(By.Id("DURATION_CODE")).Click();
            driver.FindElement(By.Id("COLLECT_FREQ_CODE")).Click();
            {
                var dropdown = driver.FindElement(By.Id("COLLECT_FREQ_CODE"));
                dropdown.FindElement(By.XPath("//option[. = 'EVERY DAY']")).Click();
            }
            driver.FindElement(By.Id("COLLECT_FREQ_CODE")).Click();
            driver.FindElement(By.Id("btnSave")).Click();
            driver.FindElement(By.LinkText("Back to Site List")).Click();
            driver.FindElement(By.LinkText("Back to List")).Click();
            //driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            //wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/SiteEdit/{siteEditId}']")).Click();
            //wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.FindElement(By.CssSelector($"[href*='/Site/MonitorEdit/{siteMonitorEditId}']")).Click();
            //BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
            //driver.FindElement(By.Id("btnSave")).Click();
            //driver.FindElement(By.LinkText("Back to List")).Click();
            //Console.WriteLine(driver.Title);
        }

        private NameValueCollection getSiteAddFieldCollection()
        {
            NameValueCollection siteAddEditFieldCollection = new NameValueCollection();
            siteAddEditFieldCollection.Add("SITE_ID", "Test_Chrome");
            siteAddEditFieldCollection.Add("SITE_NAME", "Chrome Add");
            //siteAddEditFieldCollection.Add("ORG_ID", "TEST");
            siteAddEditFieldCollection.Add("AQS_SITE_ID", "1");
            siteAddEditFieldCollection.Add("SITE_COMMENTS", "Test through BrowserStack for Chrome");
            siteAddEditFieldCollection.Add("LATITUDE", "36.19730");
            siteAddEditFieldCollection.Add("LONGITUDE", "-95.87295");
            siteAddEditFieldCollection.Add("ELEVATION", "1");
            siteAddEditFieldCollection.Add("ADDRESS", "Test Address");
            siteAddEditFieldCollection.Add("CITY", "Test City");
            siteAddEditFieldCollection.Add("STATE_CD", "34");
            siteAddEditFieldCollection.Add("COUNTY_CD", "001");
            siteAddEditFieldCollection.Add("ZIP_CODE", "12345-");
            return siteAddEditFieldCollection;
        }
        private NameValueCollection getSiteAddFieldCollectionProcessed(IWebDriver driver)
        {
            NameValueCollection siteAddEditFieldCollection = new NameValueCollection();
            //siteAddEditFieldCollection.Add("SITE_ID", "Test_Chrome");
            siteAddEditFieldCollection.Add("SITE_ID", driver.FindElement(By.Name("SITE_ID")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("SITE_NAME", driver.FindElement(By.Name("SITE_NAME")).GetAttribute("value"));
            //siteAddEditFieldCollection.Add("ORG_ID", "TEST");
            siteAddEditFieldCollection.Add("AQS_SITE_ID", driver.FindElement(By.Name("AQS_SITE_ID")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("SITE_COMMENTS", driver.FindElement(By.Name("SITE_COMMENTS")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("LATITUDE", driver.FindElement(By.Name("LATITUDE")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("LONGITUDE", driver.FindElement(By.Name("LONGITUDE")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("ELEVATION", driver.FindElement(By.Name("ELEVATION")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("ADDRESS", driver.FindElement(By.Name("ADDRESS")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("CITY", driver.FindElement(By.Name("CITY")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("STATE_CD", driver.FindElement(By.Name("STATE_CD")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("COUNTY_CD", driver.FindElement(By.Name("COUNTY_CD")).GetAttribute("value"));
            siteAddEditFieldCollection.Add("ZIP_CODE", driver.FindElement(By.Name("ZIP_CODE")).GetAttribute("value"));
            return siteAddEditFieldCollection;
        }
        private bool IsCollectionMatch(NameValueCollection addCollection,
            NameValueCollection returnedCollection)
        {
            Boolean actResult = true;
            foreach (String key in addCollection)
            {
                if (addCollection.Get(key) != returnedCollection.Get(key))
                {
                    if (key.Equals("SITE_COMMENTS")) continue;
                    if (key.Equals("ZIP_CODE"))
                    {
                        //A dash is automatically added during insert, need to fix it
                        if (returnedCollection.Get(key).EndsWith("-")) continue;
                    }
                    actResult = false;
                    break;
                }
            }
            return actResult;
        }
        private NameValueCollection getSiteMonitorAddFieldCollection()
        {
            NameValueCollection siteMonitorAddEditFieldCollection = new NameValueCollection();
            //siteMonitorAddEditFieldCollection.Add("txtParMethodTemp", "11101 - Suspended particulate (TSP) / 079");
            siteMonitorAddEditFieldCollection.Add("COLLECT_UNIT_CODE", "027"); //Beta Scatter
            siteMonitorAddEditFieldCollection.Add("POC", "102");
            siteMonitorAddEditFieldCollection.Add("DURATION_CODE", "2"); //2 HOUR
            siteMonitorAddEditFieldCollection.Add("COLLECT_FREQ_CODE", "2"); //EVERY OTHER DAY
                                                                             //siteMonitorAddEditFieldCollection.Add("ALERT_MIN_VALUE", "5");
                                                                             //siteMonitorAddEditFieldCollection.Add("ALERT_MAX_VALUE", "10");
                                                                             //siteMonitorAddEditFieldCollection.Add("ALERT_PCT_CHANGE", "6");
                                                                             //siteMonitorAddEditFieldCollection.Add("ALERT_STUCK_REC_COUNT", "6");
            return siteMonitorAddEditFieldCollection;
        }
        private NameValueCollection getSiteMonitorAddFieldCollectionPopulated(IWebDriver driver)
        {
            NameValueCollection siteMonitorAddEditFieldCollection = new NameValueCollection();
            //siteMonitorAddEditFieldCollection.Add("txtParMethodTemp", "11101 - Suspended particulate (TSP) / 079");
            siteMonitorAddEditFieldCollection.Add("COLLECT_UNIT_CODE", driver.FindElement(By.Name("COLLECT_UNIT_CODE")).GetAttribute("value"));
            siteMonitorAddEditFieldCollection.Add("POC", driver.FindElement(By.Name("POC")).GetAttribute("value"));
            siteMonitorAddEditFieldCollection.Add("DURATION_CODE", driver.FindElement(By.Name("DURATION_CODE")).GetAttribute("value"));
            siteMonitorAddEditFieldCollection.Add("COLLECT_FREQ_CODE", driver.FindElement(By.Name("COLLECT_FREQ_CODE")).GetAttribute("value"));
            //siteMonitorAddEditFieldCollection.Add("ALERT_MIN_VALUE", "5");
            //siteMonitorAddEditFieldCollection.Add("ALERT_MAX_VALUE", "10");
            //siteMonitorAddEditFieldCollection.Add("ALERT_PCT_CHANGE", "6");
            //siteMonitorAddEditFieldCollection.Add("ALERT_STUCK_REC_COUNT", "6");
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

        #region "Composite Test"

        //******************************************************************
        //Test 1: Create Site.Read the httpresponse of this test (either by reading the url you are directed to (e.g.https://qrest-test.azurewebsites.net/Site/SiteEdit/bdfc8c85-9c8a-4276-ac1f-e161a6e36b66) or reading this on the response: <input id="SITE_IDX" name="SITE_IDX" type="hidden" value="bdfc8c85-9c8a-4276-ac1f-e161a6e36b66"> to find and record the SiteIDX
        //Confirm that the various fields that were entered appear in the resposne (i.e.SITE_NAME etc)
        //Test 2: Edit Site. Using the IDX from above
        //Test 3: Create Monitor. Once again grab the monitor IDX either from the URL string (e.g. /Site/MonitorEdit/3a36a7e1-6943-4b5b-a357-2449264bdc53) or from<input id= "MONITOR_IDX" name= "MONITOR_IDX" type= "hidden" value= "3a36a7e1-6943-4b5b-a357-2449264bdc53" > on the httpresponse
        //Test 4: Edit Monitor. Using the MONITOR_IDX from above
        //Test 5: Delete Monitor. Cleanup the monitor that was just created
        //Test 6: Delete Site. Cleanup the site that was just created
        //******************************************************************

        //Respective test method will throw test fail exception, if problem occurs
        //TODO: bring in data population methods here and handle code duplication, refactor
        [TestMethod]
        public void CromeTestComposite()
        {
            IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site List, Chrome", "true", browserStackUser, browserStackKey);

            //LOGIN TO SITE
            loginToQREST(driver);
            string siteID = "";

            //CREATE NEW SITE AND VERIFY
            siteID = AddSiteTestComposite(driver, "Chrome");
            if (!string.IsNullOrEmpty(siteID))
            {
                //EDIT SITE DATA AND VERIFY
                EditSiteTestComposite(driver, "Chrome", siteID);

                //ADD NEW SITE MONITOR ENTRY AND VERIFY
                string siteMonitorID = "";
                siteMonitorID = AddSiteMonitorTestComposite(driver, "Chrome", siteID);
                if (!string.IsNullOrEmpty(siteMonitorID))
                {

                    //EDIT SITE MONITOR ENTRY AND VERIFY
                    EditSiteMonitorTestComposite(driver, "Chrome", siteMonitorID);

                    //DELETE SITE MONITOR
                    DeleteSiteMonitorTestComposite(driver, "Chrome", siteID);
                }
            }
            //DELETE SITE
            DeleteSiteTestComposite(driver, "Chrome");

            driver.Quit();
        }
        private string AddSiteTestComposite(IWebDriver driver, string browser)
        {
            string returnVal = string.Empty;
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            driver.FindElement(By.LinkText("Add")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
            BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
            driver.FindElement(By.Id("btnSave")).Click();

            //Get newly created SiteID and pass it accross test chain
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            string newSiteID = driver.Url.Substring(driver.Url.LastIndexOf("/") + 1);
            if (newSiteID.Length > 0 && newSiteID.Contains("SiteEdit") == false)
            {
                //Verify it has been added
                NameValueCollection siteAddEditFieldCollectionProcessed = getSiteAddFieldCollectionProcessed(driver);
                if (!IsCollectionMatch(siteAddEditFieldCollection, siteAddEditFieldCollectionProcessed))
                {
                    Assert.Fail("addSiteTest: Data values does not match");
                }
                returnVal = newSiteID;

            }
            else
            {
                Assert.Fail("addSiteTest: unable to get new site id!");
            }
            return returnVal;
        }
        private void EditSiteTestComposite(IWebDriver driver, string browser, string siteID)
        {

            NameValueCollection siteAddEditFieldCollection = getSiteAddFieldCollection();
            //siteAddEditFieldCollection["SITE_NAME"] = $"{browser}_Edit";

            //Set Method updates the value or adds new value to the collection
            siteAddEditFieldCollection.Set("SITE_ID", "Test_Chrome_Edit");
            siteAddEditFieldCollection.Set("SITE_NAME", "Chrome Edit");
            //siteAddEditFieldCollection.Add("ORG_ID", "TEST");
            siteAddEditFieldCollection.Set("AQS_SITE_ID", "2");
            siteAddEditFieldCollection.Set("SITE_COMMENTS", "Test through BrowserStack for Chrome - Edit");
            siteAddEditFieldCollection.Set("LATITUDE", "36.19777");
            siteAddEditFieldCollection.Set("LONGITUDE", "-95.87222");
            siteAddEditFieldCollection.Set("ELEVATION", "2");
            siteAddEditFieldCollection.Set("ADDRESS", "Test Address - Edit");
            siteAddEditFieldCollection.Set("CITY", "Test City - Edit");
            siteAddEditFieldCollection.Set("STATE_CD", "45"); //South Carolina
            siteAddEditFieldCollection.Set("COUNTY_CD", "033"); //Dillon
            siteAddEditFieldCollection.Set("ZIP_CODE", "54321-");

            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteEdit/{siteID}");
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            BrowserProperties.SetFieldValues(siteAddEditFieldCollection, ref driver);
            driver.FindElement(By.Id("btnSave")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            NameValueCollection siteAddEditFieldCollectionProcessed = getSiteAddFieldCollectionProcessed(driver);
            if (!IsCollectionMatch(siteAddEditFieldCollection, siteAddEditFieldCollectionProcessed))
            {
                Assert.Fail("editSiteTestComposite: Data values does not match");
            }

        }
        private string AddSiteMonitorTestComposite(IWebDriver driver, string browser, string siteID)
        {
            string returnVal = string.Empty;
            NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteEdit/{siteID}");
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.LinkText("Add")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.Id("add-it")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.XPath("//div[@id=\'gridData_filter\']/label/input")).SendKeys("11101");
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@data-id='54e82cab-26af-4a05-b4d8-8c44e3852da4']")));
            element.Click();

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.Id("btnSave")).Click();

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            string newSiteMonitorID = driver.Url.Substring(driver.Url.LastIndexOf("/") + 1);
            if (newSiteMonitorID.Length > 0 && newSiteMonitorID.Contains("MonitorEdit") == false)
            {
                //Verify if it is added
                NameValueCollection siteMonitorAddEditFieldCollectionPopulated = getSiteMonitorAddFieldCollectionPopulated(driver);
                if (!IsCollectionMatch(siteMonitorAddEditFieldCollection, siteMonitorAddEditFieldCollectionPopulated))
                {
                    Assert.Fail("addSiteMonitorTestReturnsNewID: Data values does not match");
                }
                returnVal = newSiteMonitorID;
            }
            return returnVal;
        }
        private void EditSiteMonitorTestComposite(IWebDriver driver, string browser, string siteMonitorID)
        {
            NameValueCollection siteMonitorAddEditFieldCollection = getSiteMonitorAddFieldCollection();
            //Change values for Edit
            siteMonitorAddEditFieldCollection.Set("COLLECT_UNIT_CODE", "076"); //Liters
            siteMonitorAddEditFieldCollection.Set("POC", "103");
            siteMonitorAddEditFieldCollection.Set("DURATION_CODE", "3"); //4 HOURS
            siteMonitorAddEditFieldCollection.Set("COLLECT_FREQ_CODE", "9"); //RANDOM
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/MonitorEdit/{siteMonitorID}");
            wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            BrowserProperties.SetFieldValues(siteMonitorAddEditFieldCollection, ref driver);
            driver.FindElement(By.Id("btnSave")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            //Get Populated Values
            NameValueCollection siteMonitorAddEditFieldCollectionPopulated = getSiteMonitorAddFieldCollectionPopulated(driver);
            //Verify if both are same, else fiail the test
            if (!IsCollectionMatch(siteMonitorAddEditFieldCollection, siteMonitorAddEditFieldCollectionPopulated))
            {
                Assert.Fail("editSiteMonitorTestWithID: Data values does not match");
            }
        }
        private void DeleteSiteMonitorTestComposite(IWebDriver driver, string browser, string siteID)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteEdit/{siteID}");
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.CssSelector(".fa-times")).Click();
            driver.FindElement(By.CssSelector(".btn > b")).Click();
            driver.FindElement(By.LinkText("Back to List")).Click();
        }
        private void DeleteSiteTestComposite(IWebDriver driver, string browser)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
            wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //delete button click
            driver.FindElement(By.XPath("//table[@id='gridData']/tbody/tr/td/div/a")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            //delete confirmation button click
            driver.FindElement(By.XPath("//table[@id='gridData']/tbody/tr/td/div/div/b")).Click();
        }


        //Temporary Test Method to run individual test - Manual
        //-----------------------------------------------------
        //[TestMethod]
        //public void RunTest()
        //{
        //    IWebDriver driver = BrowserProperties.SetTestDriver("Chrome", "75.0", "Windows", "10", "1920x1200", "QREST Site List, Chrome", "true", browserStackUser, browserStackKey);

        //    //LOGIN TO SITE
        //    loginToQREST(driver);
        //    EditSiteTestComposite(driver, "Chrome", "2c23660f-9200-4477-843c-b47fdc39e8ab");
        //    driver.Quit();

        //}

        #endregion

        //#region FireFox

        //[TestMethod]
        //public void FireFoxSiteListTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        //	driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteTest(driver, "Firefox");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Edit Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteTest(driver, "Firefox");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Delete Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteTest(driver, "Firefox");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteMonitorDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Delete Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteMonitorTest(driver, "Firefox");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteMonitorAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Add Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteMonitorTest(driver, "Firefox");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void FireFoxSiteMonitorEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Firefox", "69.0 beta", "Windows", "10", "1920x1200", "QRest Site Edit Firefox", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteMonitorTest(driver, "Firefox");
        //	driver.Quit();
        //}
        //#endregion

        //#region IE

        //[TestMethod]
        //public void IESiteListTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        //	driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteTest(driver, "IE");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteTest(driver, "IE");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteTest(driver, "IE");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteMonitorDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Delete IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteMonitorTest(driver, "IE");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteMonitorAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Add IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteMonitorTest(driver, "IE");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void IESiteMonitorEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("IE", "11.0", "Windows", "10", "1920x1200", "QRest Site Edit IE", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteMonitorTest(driver, "IE");
        //	driver.Quit();
        //}
        //#endregion

        //#region Edge

        //[TestMethod]
        //public void EdgeSiteListTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        //	driver.Navigate().GoToUrl($"{ConfigurationManager.AppSettings["testBaseUrl"]}/Site/SiteList");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteTest(driver, "Edge");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteTest(driver, "Edge");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteTest(driver, "Edge");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteMonitorDeleteTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Delete Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	deleteSiteMonitorTest(driver, "Edge");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteMonitorAddTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Add Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	addSiteMonitorTest(driver, "Edge");
        //	driver.Quit();
        //}

        //[TestMethod]
        //public void EdgeSiteMonitorEditTest()
        //{
        //	string loginUrl = $"{ConfigurationManager.AppSettings["testBaseUrl"]}{ConfigurationManager.AppSettings["QRestLoginURL"]}";
        //	IWebDriver driver = BrowserProperties.SetTestDriver("Edge", "18.0", "Windows", "10", "1920x1200", "QRest Site Edit Edge", "true", browserStackUser, browserStackKey);
        //	loginToQREST(driver);
        //	editSiteMonitorTest(driver, "Edge");
        //	driver.Quit();
        //}
        //      #endregion
    }
}
