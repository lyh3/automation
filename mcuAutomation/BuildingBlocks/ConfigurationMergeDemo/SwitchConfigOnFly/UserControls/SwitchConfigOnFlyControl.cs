using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

using log4net;
using McAfeeLabs.Engineering.Automation.Base;
using McAfeeLabs.Engineering.Automation.Profile.ModulePlugin;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace SwitchConfigOnFly
{
    public partial class SwitchConfigOnFlyControl : UserControl
    {
        ILog _logger;
        Thread _worker;
        Assembly _assembly;
        XmlDocument _sourceXmlDoc;
        IntPtr _hWndConsole;
        MainForm _maiForm;
        const string LogoutputXpath = @"/configuration/log4net/appender[@name='RollingFileAppender']/file/@value";

        public SwitchConfigOnFlyControl()
        {
            InitializeComponent();
        }

        public SwitchConfigOnFlyControl(ILog logger, MainForm maiForm)
        {
            InitializeComponent();
            _hWndConsole = MainForm.FindWindow(null, @"file:///D:/Relentless/Raiden/Trunk/Source/Cores/Libraries/ConfigurationMergeDemo/SwitchConfigOnFly/bin/Debug/SwitchConfigOnFly.EXE");
            
            _logger = logger;
            _maiForm = maiForm;
            _assembly = Assembly.GetExecutingAssembly();
            var resources = _assembly.GetManifestResourceNames();
            foreach (var resourceName in resources)
            {
                if (resourceName.EndsWith(@"config"))
                    this.listBoxConfigSource.Items.Add(resourceName);
            }

            _worker = new Thread(new ThreadStart(WriteInfo));
            _worker.Start();

            var xmlDoc = Utility.LoadAppConfigXml();
            UpdateLogoutputPathDisplay(xmlDoc);

            OnStop(null, null);
        }

        private void UpdateLogoutputPathDisplay(XmlDocument xmlDoc)
        {
            try
            {
                var node = xmlDoc.SelectSingleNode(LogoutputXpath);
                txtLogfileName.Text = node.InnerText;
            }
            catch { }
        }

        void WriteInfo()
        {
            for (; ; )
            {
                _logger.Info(@"--- Switch configuration on fly demo.");
                Thread.Sleep(1000);
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            if (_hWndConsole != IntPtr.Zero)
                MainForm.ShowWindow(_hWndConsole, 1);
        }

        private void OnStop(object sender, EventArgs e)
        {

            if (_hWndConsole != IntPtr.Zero)
                MainForm.ShowWindow(_hWndConsole, 0);

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _sourceXmlDoc = new XmlDocument();
                var sourceFileName = listBoxConfigSource.Items[listBoxConfigSource.SelectedIndex].ToString();
                if (string.IsNullOrEmpty(sourceFileName)) return;
                txtConfigSource.Text = sourceFileName;

                using (var stream = _assembly.GetManifestResourceStream(sourceFileName))
                {
                    if (null != stream)
                    {
                        var reader = new StreamReader(stream);
                        var xml = reader.ReadToEnd();
                        _sourceXmlDoc.LoadXml(xml);
                        UpdateLogoutputPathDisplay(_sourceXmlDoc);
                        btnApply.Enabled = true;
                    }
                }
            }
            catch (Exception ex) { _maiForm.DisplayMessage(@"---OnSelectedIndexChanged", ex); }
        }

        private void OnApply(object sender, EventArgs e)
        {
            try
            {
                btnApply.Enabled = false;
                if (null == _sourceXmlDoc || string.IsNullOrEmpty(_sourceXmlDoc.OuterXml)) return;
                
                UpdateLogoutput();
                _sourceXmlDoc.Save(MainForm.DefaultDynamicConfigFileName);


                var dynamicconfig = new DynamicConfiguration();

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(string.Format(@"---- Switch config to <{0}>", txtConfigSource.Text));
                Console.WriteLine("");
                Console.WriteLine("");
                dynamicconfig.SwitchConfigSource(MainForm.DefaultDynamicConfigFileName);
                MainForm.InitializeLogger();
            }
            catch (Exception ex) { _maiForm.DisplayMessage(@"---OnApply", ex); }
        }

        private void OnBrowseClick(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (_openFileDialog.FileName.Length >= 1)
                {
                    if (_openFileDialog.FileName.EndsWith(@".xml")
                        || _openFileDialog.FileName.EndsWith(@".config"))
                    {
                        try
                        {
                            _sourceXmlDoc = new XmlDocument();
                            _sourceXmlDoc.Load(_openFileDialog.FileName);
                            UpdateLogoutputPathDisplay(_sourceXmlDoc);
                            txtConfigSource.Text = _openFileDialog.FileName;
                            btnApply.Enabled = true;
                        }
                        catch (Exception ex) { _maiForm.DisplayMessage(@"---OnBrowseClick", ex); }
                    }
                }
            }
        }

        private void UpdateLogoutput()
        {
            if (null == _sourceXmlDoc 
                || string.IsNullOrEmpty(_sourceXmlDoc.OuterXml)) 
                return;
            var targetlogfile = txtLogfileName.Text.Trim();
            if (string.IsNullOrWhiteSpace(targetlogfile)) return;

            var xpathNavigatorMerger = new XpathNavigatorMerge(_sourceXmlDoc);
            var navDictionary = new Dictionary<string, string>();
            navDictionary.Add(LogoutputXpath, string.Empty);
            xpathNavigatorMerger.SelectUpdate(navDictionary, targetlogfile);
            _sourceXmlDoc.LoadXml(xpathNavigatorMerger.MergedXmlDoc.OuterXml);
        }
    }
}
