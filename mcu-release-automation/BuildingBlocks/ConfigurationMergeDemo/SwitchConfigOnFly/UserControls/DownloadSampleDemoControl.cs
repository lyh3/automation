using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

using Microsoft.Practices.Unity;

using McAfeeLabs.Engineering.Automation.Base;
using McAfeeLabs.Engineering.Automation.Base.MD5SampleBatchDownloadUnityContainerFactory;

namespace SwitchConfigOnFly
{
    public partial class DownloadSampleDemoControl : UserControl
    {
        MainForm _maiForm;
        List<string> _md5List = new List<string>();
        private IMD5BatchDowload _md5BatchDownload;
        private TreeNode _currentSelectedTreeNode;

        public DownloadSampleDemoControl()
        {
            InitializeComponent();
        }

        public DownloadSampleDemoControl(MainForm maiForm) : this()
        {
            _maiForm = maiForm;
            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"../..");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                try
                {
                    treeViewFolderExplorer.Nodes.Add(rootNode);
                    treeViewFolderExplorer.ExpandAll();
                }
                catch (Exception ex)
                {
                    _maiForm.DisplayMessage(@"---PopulateTreeView", ex);
                }
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void OnBrowseMD5ListClick(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (_openFileDialog.FileName.Length >= 1)
                {
                    if (_openFileDialog.FileName.EndsWith(@".txt"))
                    {
                        try
                        {
                            _md5List.Clear();
                            txtMD5Source.Text = _openFileDialog.FileName;
                            using (var file = new FileStream(_openFileDialog.FileName, FileMode.Open))
                            {
                                if (null != file)
                                {
                                    var reader = new StreamReader(file);
                                    string hash = string.Empty;
                                    while (!reader.EndOfStream)
                                    {
                                        hash = reader.ReadLine();
                                        if (string.IsNullOrEmpty(hash)) continue;
                                        var h = hash.ToLower().Replace(@"0x", string.Empty);
                                        if (h.IsValidMD5HashString())
                                        {
                                            listBoxMD5.Items.Add(h);
                                            _md5List.Add(h);
                                        }
                                    }
                                    file.Close();
                                }
                            }
                            btnDownload.Enabled = _md5List.Count > 0;
                        }
                        catch (Exception ex) { _maiForm.DisplayMessage(@"---OnBrowseMD5ListClick", ex); }
                    }
                }
            }
        }

        private void OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _currentSelectedTreeNode = e.Node;
            UpdateFolderView();
        }

        private void UpdateFolderView()
        {
            listViewFoder.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)_currentSelectedTreeNode.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"), 
                     new ListViewItem.ListViewSubItem(item, 
						dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listViewFoder.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"), 
                     new ListViewItem.ListViewSubItem(item, 
						file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listViewFoder.Items.Add(item);
            }

            listViewFoder.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void OnDownloadClick(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;
            _md5BatchDownload = MD5SampleBatchDownLoadUnityContainerFactory.Instance.Resolve<IMD5BatchDowload>();
            _md5BatchDownload.Uri = ConfigurationManager.AppSettings["FileServiceUri"];
            _md5BatchDownload.BatchComplete += OnBatchDowloadComplete;
            _md5BatchDownload.SampleBatchDownload(_md5List, @"D:\Relentless\Raiden\Trunk\Source\Cores\Libraries\ConfigurationMergeDemo\SwitchConfigOnFly\Download", @"zip");
            var worker = new Thread(() => DownladSamples());
            worker.Start();
        }

        void DownladSamples()
        {
            _md5BatchDownload.SampleBatchDownload(_md5List, @"D:\Relentless\Raiden\Trunk\Source\Cores\Libraries\ConfigurationMergeDemo\SwitchConfigOnFly\Download", @"zip");
        }

        void OnBatchDowloadComplete(object sender, EventArgs args)
        {
            //_md5BatchDownload.BatchComplete -= OnBatchDowloadComplete;
            //_md5BatchDownload.CurrentBatcMD5hList.ForEach(md5 =>
            //{
            //});
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (null == _currentSelectedTreeNode) return;
            UpdateFolderView();
        }
    }
}
