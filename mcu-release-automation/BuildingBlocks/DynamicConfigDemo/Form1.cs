using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;

using McAfeeLabs.Engineering.Automation.Base;

namespace DynamicConfigDemo
{
    public partial class Form1 : Form
    {
        private ConfigurationChangeMonitor _configchangemonitor;
        protected ColorConfigProperty _colorConfigProperty;
        protected StringConfigProperty _submissionGroupProperty;
        protected IntConfigProperty _maxfilesizeProperty;

        private static SolidBrush _brush;
        private static string _submissiongroup;
        private static int _maxfilesize;

        private static Rectangle _rectangle;
        private static bool _isDirty = false;
        public Form1()
        {
            InitializeComponent();

            InitializeConfigChangeMonitor();
            _configchangemonitor.LazyUpdate = false;

            InitializeGuis();

            UpdateGui();
        }

        private void InitializeConfigChangeMonitor()
        {
            //appSettings section
            _configchangemonitor = new ConfigurationChangeMonitor(Utility.GetExecutedPath());
            var xpath = string.Format(XmlConfigProperty.AppSettingsKeyXpathFormat, "Color");
            _colorConfigProperty = new ColorConfigProperty { XPath = xpath };
            _configchangemonitor.Add(_colorConfigProperty);

            //Custom section
            _submissionGroupProperty = new StringConfigProperty { XPath = @"/*[local-name()='configuration']/*[local-name()='SubmisssionParameterRoot']/*[local-name()='SubmissionGroup']" };
            _configchangemonitor.Add(_submissionGroupProperty);
            _maxfilesizeProperty = new IntConfigProperty { XPath = @"/*[local-name()='configuration']/*[local-name()='SubmisssionParameterRoot']/*[local-name()='MaxFileSize']" };
            _configchangemonitor.Add(_maxfilesizeProperty);

            _configchangemonitor.PropertyChanged += OnCofigPropertyChanged;

        }

        private void InitializeGuis()
        {
            _rectangle = new Rectangle
            {
                Location = new Point(5, 5),
                Size = new Size(130, 100),
            };

            checkBoxLazyUpdate.Checked = _configchangemonitor.LazyUpdate;
            OnCheckedChanged(this, null);

            this.Paint += new PaintEventHandler(OnPaint);
        }

        void OnCofigPropertyChanged(object source, System.EventArgs args)
        {
            var value = _configchangemonitor[_colorConfigProperty.Id];
            if(null != value)
                _brush = (SolidBrush)value;
            value = _configchangemonitor[_submissionGroupProperty.Id];
            if (null != value)
                _submissiongroup = (string)value;
            value = _configchangemonitor[_maxfilesizeProperty.Id];
            if (null != value)
                _maxfilesize = (int)value;

            _isDirty = true;
        }

        private void OnSyncClick(object sender, EventArgs e)
        {
            _configchangemonitor.RequestUpdate();
        }

        void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(_brush, _rectangle);
            if (_isDirty)
            {
                this.Refresh();
                _isDirty = false;
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            btnSync.Enabled = _configchangemonitor.LazyUpdate = checkBoxLazyUpdate.Checked;
        }

        private void UpdateGui()
        {
            var strcolor = ConfigurationManager.AppSettings["Color"];
            if (string.IsNullOrEmpty(strcolor)) return;
            var color = Enum.Parse(typeof(KnownColor), strcolor);

            _brush = new SolidBrush(Color.FromName(strcolor));

            var xmldoc = Utility.LoadAppConfigXml();
            var xmlnode = xmldoc.SelectSingleNode(@"/*[local-name()='configuration']/*[local-name()='SubmisssionParameterRoot']/*[local-name()='SubmissionGroup']");
            _submissiongroup = xmlnode.InnerText;
            xmlnode = xmldoc.SelectSingleNode(@"/*[local-name()='configuration']/*[local-name()='SubmisssionParameterRoot']/*[local-name()='MaxFileSize']");
            _maxfilesize = int.Parse(xmlnode.InnerText);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_isDirty)
                this.Refresh();
            textSubmissionGroup.Text = _submissiongroup;
            textMaxFileSize.Text = _maxfilesize.ToString();
        }
    }

    public class ColorConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            return new SolidBrush(Color.FromName(node.InnerText)); 
        }
    }
}
