using System;
using System.Windows.Forms;
using System.Xml;

using McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel;
using log4net;

namespace SwitchConfigOnFly
{
    public partial class DynamicMergeControl : UserControl
    {
        XmlDocument _mergedXmlDoc = null;
        MainForm _maiForm;
        public DynamicMergeControl(MainForm maiForm)
        {
            InitializeComponent();
            _maiForm = maiForm;
        }

        private void OnBrowseMergeScriptClick(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (_openFileDialog.FileName.Length >= 1)
                {
                    if (_openFileDialog.FileName.EndsWith(@".xml")
                        || _openFileDialog.FileName.EndsWith(@".config"))
                    {
                        this.txtMergeScript.Text = mergeScriptSourceTextBox.Text = _openFileDialog.FileName;
                    }
                }
            }
        }

        private void OnBrowseMergeTargetClick(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (_openFileDialog.FileName.Length >= 1)
                {
                    if (_openFileDialog.FileName.EndsWith(@".xml")
                        || _openFileDialog.FileName.EndsWith(@".config"))
                    {
                        this.txtMergeTarget.Text = xmlTreeViewTarget.XmlFile = _openFileDialog.FileName;
                    }
                }
            }
        }

        private void OnMergeClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(mergeScriptSourceTextBox.Text) || string.IsNullOrEmpty(xmlTreeViewTarget.Xml))
                    return;
                var targetXmlDoc = new XmlDocument();
                targetXmlDoc.LoadXml(xmlTreeViewTarget.Xml);
                var sourceXmlDoc = new XmlDocument();
                sourceXmlDoc.LoadXml(mergeScriptSourceTextBox.Text);

                var mergeworker = new MergeWorker(_maiForm.Logger, sourceXmlDoc, targetXmlDoc);
                mergeworker.Merge();
                _mergedXmlDoc = mergeworker.MergedXmlDoc;
               
                xmlTreeViewTarget.Xml = _mergedXmlDoc.OuterXml;
            }
            catch (Exception ex)
            {
                _maiForm.DisplayMessage(@"---OnMergeClick", ex);
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            if (null == _mergedXmlDoc || string.IsNullOrEmpty(_mergedXmlDoc.OuterXml))
                return;
            _mergedXmlDoc.Save(MainForm.DefaultDynamicConfigFileName);
        }
    }
}
