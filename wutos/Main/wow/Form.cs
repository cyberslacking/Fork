using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

using Nancy.Hosting.Self;

using CefSharp.WinForms;
using CefSharp;

using Metro;

namespace wow
{
    public partial class Form : System.Windows.Forms.Form
    {
        private NancyHost nancyHost;
        private ChromiumWebBrowser browser;


        private string path = AppDomain.CurrentDomain.BaseDirectory;


        public Form()
        {
            InitializeComponent();
            
            menuFolder.Elements = new List<MetroMenuElement>()
            {
                new MetroMenuElement()
                {
                    Title = "接入设备",
                    Icon = new Bitmap(path + "images\\MetroDevice.png"),
                },
                new MetroMenuElement()
                {
                    Title = "防区",
                    Icon = new Bitmap(path + "images\\MetroDefence.png"),
                },
                new MetroMenuElement()
                {
                    Title = "传感器",
                    Icon = new Bitmap(path + "images\\MetroSensor.png"),
                },
                new MetroMenuElement()
                {
                    Title = "报警",
                    Icon = new Bitmap(path + "images\\MetroSensor.png"),
                },
                new MetroMenuElement()
                {
                    Title = "故障",
                    Icon = new Bitmap(path + "images\\MetroSensor.png"),
                },
            };
            menuFolder.ElementSelected += MenuFolder_ElementSelected;
        }

        private void MenuFolder_ElementSelected(object sender, MenuElementEventArgs e)
        {
            MessageBox.Show(this, e.Element.Title);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作
                return cp;
            }
        }

        void Init()
        {
            #region nancy http
            string port = "9000";
            if (ConfigurationManager.AppSettings["NancyPort"] != null)
            {
                port = ConfigurationManager.AppSettings["NancyPort"];
            }
            string url = string.Format("http://localhost:{0}/", port);
            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            nancyHost = new NancyHost(hostConfigs, new Uri(url));
            nancyHost.Start();
            #endregion

            #region CefSharp
            Cef.EnableHighDPISupport();
            browser = new ChromiumWebBrowser(url);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true; //新cefsharp绑定需要优先申明
            browser.RegisterJsObject("webForm", new WebForm(), new BindingOptions { CamelCaseJavascriptNames = false });
            Model model = new Model { name = "name", number = 1 };
            browser.RegisterJsObject("model", model, new BindingOptions { CamelCaseJavascriptNames = false });
            browser.Dock = DockStyle.Fill;
            panelBrowser.Controls.Add(browser);
            browser.FrameLoadEnd += browser_FrameLoadEnd;
            #endregion

        }

        void UnInit()
        {
            nancyHost.Stop();
            browser.Dispose();
            Cef.Shutdown();
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {

        }

        private void Form_Load(object sender, EventArgs e)
        {
            /*
            string computer = ComputerInfo.GetComputerInfo();
            string encryptComputer = new EncryptionHelper().EncryptString(computer);

            if (CheckRegist(encryptComputer) == true)
            {
                Text += "已注册";
            }
            */
            Init();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnInit();
        }

        private void buttonNavigation_Click(object sender, EventArgs e)
        {
            foreach(var o in panelNavigation.Controls)
            {
                if (o == sender)
                {

                }
            }
        }

        /*
        private bool CheckRegist(string encryptComputer)
        {
            EncryptionHelper helper = new EncryptionHelper();
            string md5key = helper.GetMD5String(encryptComputer);
            return CheckRegistData(md5key);
        }

        private bool CheckRegistData(string key)
        {
            if (RegistFileHelper.ExistRegistInfofile() == false)
            {
                return false;
            }
            else
            {
                string info = RegistFileHelper.ReadRegistFile();
                var helper = new EncryptionHelper(EncryptionKeyEnum.KeyB);
                string registData = helper.DecryptString(info);
                if (key == registData)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        */
    }

    public class Model
    {
        public string name;
        public int number;
    }

    public class WebForm
    {
        private delegate void ShowVideo3DEventHandler(string id);
        public void Invoke(string text)
        {
            MessageBox.Show(text);
        }
        public void InvokeCamera(string id)
        {
        }
    }
}
