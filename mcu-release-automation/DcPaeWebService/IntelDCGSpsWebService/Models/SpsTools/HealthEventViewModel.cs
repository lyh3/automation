using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace IntelDCGSpsWebService.Models
{
    public class HealthEventViewModel : SpsToolsModel
    {
        private List<response> _responses = new List<response>();
        private string _errorMessage = string.Empty;
        public string eventType { get; set; }
        public HealthEventViewModel()
        {
            base._title = "Health Event Decode";
        }
        public response[] responsedatas
        {
            get { return _responses.ToArray(); }
            set 
            {
                if (null != value)
                {
                    _responses.Clear();
                    _responses.AddRange(value);
                }
            }
        }
        public string ErrorMessage 
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
    }
    public class response
    {
        private List<position> _positions = new List<position>();
        public string description { get; set; }
        public string dependence { get; set; }
        public Boolean hex { get; set; }
        public position[] positions
        {
            get { return _positions.ToArray(); }
            set { _positions.Clear(); _positions.AddRange(value); }
        }
    }
    public class position
    {
        private List<valueData> _values = new List<valueData>();
        public string pos { get; set; }
        public valueData[] values
        {
            get { return _values.ToArray(); }
            set
            {
                _values.Clear(); _values.AddRange(value);
            }
        }
    }
    public class valueData
    {
        private List<valueData> _values = new List<valueData>();
        public string value { get; set; }
        public string description { get; set; }
        public valueData[] values
        {
            get { return _values.ToArray(); }
            set
            {
                _values.Clear(); _values.AddRange(value);
            }
        }
    }
}