using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class IpmiResetModel
    {
        private DeviceType _deviceType;
        private CommandType _systemRestType;
        private ProcessStatus _processStatus;
        private Dictionary<CommandType, string> _commandDictionary = new Dictionary<CommandType, string>();
        private bool _isRemote = true;
        private BMCConfig _bmcConfig = null;
        private SutConfig _sutConfig = null;
        private CommandMaps _commandMaps = null;
        private int _repeat = 1;
        private string _configJson = string.Empty;
        private int _timeoutInMinutes = 5;
        private string _customerDefinedCommand = string.Empty;
        private string _errorMessage = string.Empty;
        private string _ipmiResponse = string.Empty;
        private CommandDoc _selectCommandDoc = null;
        private int _delayInseconds = 2;

        public IpmiResetModel()
        {
            _bmcConfig = new BMCConfig();
            _sutConfig = new SutConfig();
            _processStatus = ProcessStatus.ShutDown;
            _commandDictionary.Add(CommandType.Warm, "chassis power reset");
            _commandDictionary.Add(CommandType.AcCycle, "chassis power cycle");
            _commandDictionary.Add(CommandType.MeCold, "-b 6 -t 0x2c raw 6 2");
            _commandDictionary.Add(CommandType.CustomerDefined, _customerDefinedCommand);
        }
        [Display(Name = "Timeout (min):")]
        public int TimeoutInMimutes
        {
            get { return _timeoutInMinutes; }
            set { _timeoutInMinutes = value; }
        }
        [Display(Name = "Repeat Count:")]
        public int CurrentCount { get; set; }
        public string ErrorMessage 
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        public string IpmiResponse
        {
            get { return _ipmiResponse; }
            set { _ipmiResponse = value; }
        }
        public bool UserCanceled { get; set; }
        public string IpmiCommand { get { return _commandDictionary[_systemRestType]; } }
        public bool CanSubmit
        {
            get
            {
                var cansubmit = null != _bmcConfig && _bmcConfig.CanSubmit && _repeat > 0;
                if (_systemRestType == CommandType.CustomerDefined)
                {
                    cansubmit &= !string.IsNullOrEmpty(_customerDefinedCommand);
                }
                else
                {
                    cansubmit &= null != _sutConfig
                                && !string.IsNullOrEmpty(_sutConfig.IpAddress)
                                && Utils.IsValidIpV4(_sutConfig.IpAddress);
                }
                return cansubmit;
            }
        }
        public string DeviceType
        {
            get { return _deviceType.ToString(); }
            set { _deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), value); }
        }
        public string RestType
        {
            get { return _systemRestType.ToString(); }
            set
            {
                _systemRestType = (CommandType)Enum.Parse(typeof(CommandType), value);
                _selectCommandDoc = null;
            }
        }
        public string Status
        {
            get { return _processStatus.ToString(); }
            set { _processStatus = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), value); }
        }
        public bool IsRemote
        {
            get { return _isRemote; }
            set { _isRemote = value; }
        }
        public BMCConfig BMCCfig
        {
            get { return _bmcConfig; }
            set { _bmcConfig = value; }
        }
        public SutConfig SutCfig
        {
            get { return _sutConfig; }
            set { _sutConfig = value; }
        }
        [Display(Name = "Repeat:")]
        public string Repeat
        {
            get { return _repeat.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                string s = value.Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    int.TryParse(s, out _repeat);
                }
            }
        }
        //[Required(ErrorMessage = "* Please select config json file to load.")]
        [Display(Name = "Config Json:")]
        public string ConfigJson
        {
            get { return _configJson; }
            set { _configJson = value; }
        }
        [Display(Name = "Customer Defined:")]
        public string CustomerDefinedCommand
        {
            get { return _customerDefinedCommand; }
            set 
            {
                _customerDefinedCommand = value;
                _selectCommandDoc = null;
                if (!string.IsNullOrEmpty(_customerDefinedCommand) && null != _commandMaps)
                {
                    var command = _commandMaps.CommandMapList.FirstOrDefault<CommandMap>(x => x.IpmiCommand.StartsWith(_customerDefinedCommand.Trim()));
                    if (null != command)
                    {
                        if (null != command && !string.IsNullOrEmpty(command.CommandCode))
                        {
                            _selectCommandDoc = _commandMaps.CommandDocList.FirstOrDefault<CommandDoc>(x => x.CommandCode == command.CommandCode);
                        }
                    }
                }
                this._commandDictionary[CommandType.CustomerDefined] = value;
            }
        }
        [Display(Name = "Delay (sec)")]
        public string Delay
        {
            get { return _delayInseconds.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    int.TryParse(value, out _delayInseconds);
            }
        }
        public CommandMaps CmdMaps
        {
            get { return _commandMaps; }
            set { _commandMaps = value; }
        }

        public string FormatCustomerDefinedIpmiResponse(string command, string response)
        {
            var msg = string.Empty;
            var builder = new StringBuilder();
            if (command == "-b 6 -t 0x2c raw 0x30 0x26 0x57 1 0 4 6 2 0xb0 0x80")//"Get IDLM PID"
            {
                var split = new List<string>(response.Split(' '));
                for (var x = 0; x < 10; ++x)
                    split.RemoveAt(0);
                foreach (var x in split)
                {
                    builder.Append(string.Format("{0} ", x.Replace("\r\n", string.Empty)));
                }
                msg = builder.ToString();
            }
            else if (command == "-b 6 -t 0x2c raw 0x30 0x26 0x57 0x01 0x00 0x04 0x06 0x04 0x00 0x00 0x01 0x01")//"Get version"
            {
                var data = (response.Replace("\r\n", string.Empty).ToUpper().Split(' ')).Reverse().ToArray();
                builder.AppendLine("<table style='width:100%;font-size:small;color:gray;'>");
                builder.AppendLine("<tr>");
                builder.AppendLine(string.Format("<td><span>Protocal Version Major</span></td><td><span>{0}<span></td>",data[13]));
                builder.AppendLine("</tr>");
                builder.AppendLine("<tr>");
                builder.AppendLine(string.Format("<td><span>Protocal Version Minor</span></td><td><span>{0}<span></td>",data[12]));
                builder.AppendLine("</tr>");
                builder.AppendLine("<tr>");
                builder.AppendLine(string.Format("<td><span>Firmware Status</span></td><td><span>{0}{1}{2}{3}<span></td>",data[8], data[9], data[10], data[11]));
                builder.AppendLine("</tr>");
                builder.AppendLine("<tr>");
                builder.AppendLine(string.Format("<td><span>Extended Firmware Status</span></td><td><span>{0}{1}{2}{3}<span></td>", data[4], data[5], data[6], data[7]));
                builder.AppendLine("</tr>");
                builder.AppendLine("<tr>");
                builder.AppendLine(string.Format("<td><span>Uptime</span></td><td><span>{0}{1}{2}{3}<span></td>", data[0], data[1], data[2], data[3]));
                builder.AppendLine("</tr>");
                builder.AppendLine("</table>");
                msg = builder.ToString();
            }
            else
            {
                msg = response.Replace("\r\n", "<br/>");
            }
            return msg;
        }
        public string RequestDoc{get { return null == _selectCommandDoc ? string.Empty:_selectCommandDoc.Request; }}
        public string ResponseDoc { get { return null == _selectCommandDoc ? string.Empty : _selectCommandDoc.Response; } }
        public string CommandCode { get { return null == _selectCommandDoc ? string.Empty:_selectCommandDoc.CommandCode; }}

        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}