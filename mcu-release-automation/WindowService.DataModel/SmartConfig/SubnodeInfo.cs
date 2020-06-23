 namespace WindowService.DataModel
{
    public class SubnodeInfo
    {
        private string _subnodeHtml = string.Empty;
        private string _selectedNextSubNodePath = string.Empty;
        public string SubnodeHtml
        {
            get { return _subnodeHtml; }
            set { _subnodeHtml = value; }
        }
        public string SelectedNextSubNodePath
        {
            get { return _selectedNextSubNodePath; }
            set { _selectedNextSubNodePath = value; }
        }
    }
}
