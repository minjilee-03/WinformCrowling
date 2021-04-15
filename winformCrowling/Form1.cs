using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winformCrowling
{
    public partial class Form1 : Form
    {
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        private ChromeDriver _driver = null;

        public Form1()
        {
            InitializeComponent();

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = tboxID.Text;
            string pw = tboxPW.Text;

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://www.naver.com");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //*[@id="account"]/a
            var element = _driver.FindElementByXPath("//*[@id='account']/a");
            element.Click();

            Thread.Sleep(3000);

            element = _driver.FindElementByXPath("//*[@id='id']");
            element.SendKeys(id);

            element = _driver.FindElementByXPath("//*[@id='pw']");
            element.SendKeys(pw);

            element = _driver.FindElementByXPath("//*[@id='log.login']");
            element.Click();
        }

        List<string> Lsrc = null;
        int i = 0; 
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strURL = "https://www.google.com/search?q=" + tboxSearch.Text + "&source=Inms&tbm=isch";

            _driver = new ChromeDriver(_driverService, _options);

            _driver.Navigate().GoToUrl(strURL);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _driver.ExecuteScript("window.scrollBy(0, 10000)");

            Lsrc = new List<string>();

            foreach (IWebElement item in _driver.FindElementsByClassName("rg_i"))
            {
                if (item.GetAttribute("src") != null)
                    Lsrc.Add(item.GetAttribute("src"));
            }

            lblTotal.Text = "/ " + Lsrc.Count.ToString();

            this.Invoke(new Action(delegate ()
            {
                try
                {
                    foreach (string strsrc in Lsrc)
                    {

                        i++;

                        GetMapImage(Lsrc[i]);
                        tboxGo.Text = i.ToString();
                        Refresh();
                        Thread.Sleep(50);
                    }
                }
                catch (Exception)
                {

                }
            }));
        }

        private void GetMapImage(string base64String)
        {
            try
            {
                var base64Data = Regex.Match(base64String, @"data:image/(?<type>>, +?),(?<data>,+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    if (stream.Length == 0)
                    {
                        pboxMain.Load(base64String);
                        tboxGo.Text = i.ToString();
                        tboxUrl.Text = base64String;
                    }
                    else
                    {
                        var image = Image.FromStream(stream);
                        pboxMain.Image = image;
                        tboxUrl.Text = base64String;
                    }
                }
            }
            catch
            {

            }
        }
        private void btnPre_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i--;

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i = int.Parse(tboxGo.Text);

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i++;

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));
        }

        private void tboxPW_TextChanged(object sender, EventArgs e)
        {
            tboxPW.PasswordChar = '*';
        }
    }
}