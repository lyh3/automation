using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

using log4net;

namespace SwitchConfigOnFly
{
    public partial class MainForm : Form
    {
        public const string DefaultDynamicConfigFileName = @".\Dynamic.config";
        static ILog _logger;
        public MainForm()
        {
            InitializeComponent();
            AllocConsole();
        }

        public ILog Logger { get { return _logger; } }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeLogger();

            var switchConfigOnFlyControl = new SwitchConfigOnFlyControl(_logger, this);
            switchConfigOnFlyControl.Dock = DockStyle.Fill;
            var tabpageSwitchConfigOnFly = new TabPage("SwitchConfigOnFly");
            tabpageSwitchConfigOnFly.Controls.Add(switchConfigOnFlyControl);
            tabpageSwitchConfigOnFly.BackColor = Color.LightGray;
            this.tabControl1.TabPages.Add(tabpageSwitchConfigOnFly);

            var dynamicMergeControl = new DynamicMergeControl(this);
            dynamicMergeControl.Dock = DockStyle.Fill;
            var tabpageDynamicMerge = new TabPage("DynamicMerge");
            tabpageDynamicMerge.BackColor = Color.LightGray;
            tabpageDynamicMerge.Controls.Add(dynamicMergeControl);
            this.tabControl1.TabPages.Add(tabpageDynamicMerge);

            var downloadSampleDemoControl = new DownloadSampleDemoControl(this);
            downloadSampleDemoControl.Dock = DockStyle.Fill;
            var tabpageDownloadDemo = new TabPage("downloadSampleDemo");
            tabpageDownloadDemo.BackColor = Color.LightGray;
            tabpageDownloadDemo.Controls.Add(downloadSampleDemoControl);
            this.tabControl1.TabPages.Add(tabpageDownloadDemo);
        }

        public static void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(typeof(MainForm));
        }

        public void DisplayMessage(string method,
                                    Exception ex = null,
                                    string message = null)
        {
            var msg = string.Empty;
            messageBox.ForeColor = null != ex ? Color.Red : Color.Wheat;
            if (!string.IsNullOrEmpty(message))
                messageBox.AppendText(string.Format(@"{0}{1}", message, Environment.NewLine)); _logger.Info(message);
            if (null != ex)
            {
                msg = string.Format(@"Exception caught @ <{0}>,{1}", method, Environment.NewLine);
                messageBox.AppendText(msg); _logger.Error(msg);
                msg = string.Format(@"{0}{1}", null != ex.InnerException ? ex.InnerException.Message : ex.Message, Environment.NewLine);
                messageBox.AppendText(msg); _logger.Error(msg);
                messageBox.AppendText(Environment.NewLine);
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
