using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IntelDCGSpsWebService.Models;
using IntelDCGSpsWebService.Models.Buildingblocks;
using CookComputing.XmlRpc;
using WindowService.DataModel;

namespace DcPaeWebServiceUnitest
{
    [TestFixture]
    public class Unitest
    {
        const string JSON_FILE = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\HelathEvent.json";
        const string sampleSystemResetJsonfileName = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\IpmiWarmReset.json";
        private HealthEventViewModel _healthEventViewModel = new HealthEventViewModel();
        [SetUp]
        public void Init()
        {
        }
        [Test]
        public void XmlRpcTest()
        {
            var b = Utils.IsValidIpV4("10.23.60.7");
            var proxy = XmlRpcProxyGen.Create<IXmlRpcInterface>();
            var response = proxy.PingHost("10.23.60.7");
        }
        [Test]
        public void IpmiCommandTest()
        {
            IpmiResetModel model;
            using (StreamReader file = File.OpenText(sampleSystemResetJsonfileName))
            {
                var serializer = new JsonSerializer();
                model = (IpmiResetModel)serializer.Deserialize(file, typeof(IpmiResetModel));
            }
            model.RestType = "MeCold";
            var ipmitoolPath = "D:\\IpmiTool-1.8.11.i4-win";
            var count = 0;
            int.TryParse(model.Repeat, out count);
            for (var idx = 0; idx < count; ++idx)
            {
                var ipmiCommand = string.Format(@" -I lanplus -H {0} -U {1} -P {2} {3} {4} ",
                                                model.BMCCfig.BmcIpAddress,
                                                model.BMCCfig.User,
                                                model.BMCCfig.Password,
                                                "-b 6 -t 0x2c",
                                                model.IpmiCommand);
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WorkingDirectory = ipmitoolPath,
                    FileName = "ipmitool.exe",
                    Arguments = ipmiCommand
                };
                try
                {
                    var process = new Process() { StartInfo = startInfo };
                    process.Start();
                }
                catch (Exception)
                {

                }
            }
        }
        [Test]
        public void CommandMapTest()
        {
            var commandMapList = new List<CommandMap>();
            var maps = new []{
                new CommandMap{
                    Name = "Get Device ID",
                    IpmiCommand = "-b 6 -t 0x2c raw 6 1"
                },
                new CommandMap{
                    Name = "Get Self Test Results",
                    IpmiCommand = "-b 6 -t 0x2c raw 6 4"
                },
                new CommandMap{
                    Name = "Force Intel(R) ME Recovery",
                    IpmiCommand = "-b 6 -t 0x2c raw 0x2e 0xDF 0x57 1 0 1"
                },
                new CommandMap{
                    Name = "Force Intel(R) ME Operational",
                    IpmiCommand = "-b 6 -t 0x2c raw 0x2e 0xDF 0x57 1 0 2"
                },
                new CommandMap{
                    Name = "Get Node Manager Capabilities (C9h)",
                    IpmiCommand = "-b 0x06 -t 0x2c raw 0x2e 0xc9 0x57 0x01 0 0 16"
                },
                new CommandMap{
                    Name = "Get Node Manager Capabilities (61h)",
                    IpmiCommand = "-b 0x06 -t 0x2c raw 0x2e 0x61 0x57 0x01 0x00 0x01"
                },
                new CommandMap{
                    Name = "Get Node Manager Statistics  (C8h)",
                    IpmiCommand = " -b 6 -t 44 raw 0x2e 0xc8 87 1 0 1 0 1 "
                },
                new CommandMap{
                    Name = "Get SEL log",
                    IpmiCommand = "sel list "
                },
                new CommandMap{
                    Name = "Clear SEL log",
                    IpmiCommand = "sel clear "
                }
            };
            commandMapList.AddRange(maps);
            var commandMaps = new CommandMaps{
                CommandMapList = commandMapList
            };
            var fileName = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\IpmiCommandList.json";
            var json = commandMaps.ToString();
            Assert.True(!string.IsNullOrEmpty(json));
            File.WriteAllText(fileName, json);
            using (StreamReader file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                var m = (CommandMaps)serializer.Deserialize(file, typeof(CommandMaps));
                Assert.True(null != m);
            }
        }
        [Test]
        public void IpmiConfigTest()
        {
            var sampleIpmiConfig = new IpmiResetModel
            {
                BMCCfig = new BMCConfig
                {
                    BmcIpAddress = "10.23.60.2",
                    User = "root",
                    Password = "superuser"
                },
                SutCfig = new SutConfig
                {
                    IpAddress = "10.23.60.24"
                },
                Repeat = "1",
            };
            var json = sampleIpmiConfig.ToString();
            Assert.True(!string.IsNullOrEmpty(json));
            File.WriteAllText(sampleSystemResetJsonfileName, json);
            using (StreamReader file = File.OpenText(sampleSystemResetJsonfileName))
            {
                var serializer = new JsonSerializer();
                var model = (IpmiResetModel)serializer.Deserialize(file, typeof(IpmiResetModel));
                Assert.True(null != model);
            }
        }
        [Test]
        public void InvokePythonTest()
        {
            var path = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\Upload\";
            var commadParameters = string.Format(@" -s {5}{0}{5} -u {5}{1}{5} -o {5}{2}{5} -m {5}{3}{5} -r {5}{4}{5}",
                                    path + "source.bios",
                                    path + "replace.bin",
                                    path + "unitest.bin",
                                    path + "outimage.map",
                                    "ME Region",
                                    '"');
            var b = System.IO.File.Exists(path + "source.bios");
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = path;
            startInfo.FileName = "python";
            var argus = string.Format("ImageRegionUpdate.py {0}", commadParameters);
            startInfo.Arguments = argus;
            var process = new Process() { StartInfo = startInfo };
            process.Start();

        }

        [Test]
        public void SerialzeTest()
        {
            using (StreamReader file = File.OpenText(JSON_FILE))
            {
                var serializer = new JsonSerializer();
                var model = (HealthEventViewModel)serializer.Deserialize(file, typeof(HealthEventViewModel));
                Assert.True(null != model);
            }
        }

        [Test]
        public void HealthEventDecodeTest()
        {
            var results = utils.DecodeEventData(JSON_FILE, "a01301", "value code - {0} : description - {1}");
            Assert.True(null != results);
            Assert.AreEqual(4, results.Length);
        }

        [Test]
        //[Ignore("skip DeserializeTest")]
        public void DeserializeTest()
        {
            _healthEventViewModel = new HealthEventViewModel
            {
                eventType = "Intel® ME Firmware Health Event",
                responsedatas = new response[] { 
                    new response {
                        description = "Byte 5 – Event Data 1",
                        dependence = "",
                        hex = false,
                        positions = new position[]{
                            new position{
                                pos = "[6:7]",
                                values = new valueData[]{
                                    new valueData{
                                        value = "10b",
                                        description = "OEM code in byte 2"
                                    }
                                }
                            },
                        }
                    },
                    new response {
                        description = "Byte 6 – Event Data 2",
                        dependence = "",
                        hex = true,
                        positions = new position[]{
                            new position{
                                pos = "[0:7]",
                                values = new valueData[]{
                                    new valueData{
                                        value = "00",
                                        description = "Recovery GPIO forced. Recovery Image loaded due to recovery MGPIO pin asserted. Pin number is configurable in factory presets, Default recovery pin is MGPIO1. Repair action: Deassert MGPIO1 and reset the Intel® ME"
                                    },
                                    new valueData{
                                        value = "01",
                                        description = "Image execution failed. Recovery Image or backup operational image loaded because operational image is corrupted. This may be either caused by Flash device corruption or failed upgrade procedure. Repair action: Either the Flash device must be replaced (if error is persistent) or the upgrade procedure must be started again."
                                    },
                                    new valueData{
                                        value = "02",
                                        description = "Flash erase error. Error during Flash erasure procedure probably due to Flash part corruption. Repair action: The Flash device must be replaced."
                                    }, 
                                    new valueData{
                                        value = "03",
                                        description = "Flash state information. Repair action: Check extended info byte in Event Data 3 (byte 7) whether this is wear-out protection causing this event. If so just wait until wear-out protection expires, otherwise probably the flash device must be replaced (if error is persistent)."
                                    },
                                    new valueData{
                                        value = "04",
                                        description = "Internal error. Error during firmware execution – Repair action: Firmware should automatically recover from error state. If error is persistent then operational image shall be updated or hardware board repair is needed."
                                    },
                                    new valueData{
                                        value = "05",
                                        description = "BMC did not respond correctly to Chassis Control - Power Down command triggered by Intel® Node Manager policy failure action and Intel® ME forced shutdown. Repair action: Verify the Intel® Node Manager policy configuration."
                                    },
                                    new valueData{
                                        value = "06",
                                        description = "Direct Flash update requested by the BIOS. Intel® ME Firmware will switch to recovery mode to perform full update from BIOS. Repair action: This is transient state. Intel® ME Firmware should return to operational mode after successful image update performed by the BIOS."
                                    },
                                }
                            },
                        }
                    },
                }
            };
            var json = new JsonFormatter(JsonConvert.SerializeObject(_healthEventViewModel)).Format();
            Assert.True(!string.IsNullOrEmpty(json));
            //File.WriteAllText(JSON_FILE, json);
        }

        [Test]
        public void SmartConfigJsonTest()
        {
            var model = new SmartConfigDataModel
            {
                Root = {
                    new ConfigTreeNode {
                        Key = @"Edk2Manu",
                        Annotation = @"EDKII Menu",
                        SubMenu =
                        {
                            new ConfigTreeNode
                            {
                                Key = @"Socket",
                                Annotation = @"Socket Configuration",
                                Properties =
                                {
                                    pType = @"Hex",
                                    Options = {"0xe0", "0xe1", "0xe2", "0xe3" },
                                    Default = "0xe0",
                                    CurrentValue = "0xe0"
                                },
                                UpdateDictionary = new UpdateMatrix
                                {
                                    UpdateData =
                                    {
                                        new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("0")),
                                        new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("1"){
                                            Properties = new Props{
                                                pType = "Hex",
                                                Options = {"0xe0", "0xe1", "0xe2", "0xe3" },
                                                Default = "0xe3",
                                                CurrentValue = "0xe3"
                                            },
                                            RawDataMap = new RawData{
                                                Offset = "0x200",
                                                Size ="0x02",
                                                Value ="0x02"
                                            }
                                        }),
                                        new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("2")),
                                    }
                                },
                                RawDataMap =
                                {
                                    Offset = "0x100",
                                    Size = "0x01",
                                    Value = "0x01"
                                },
                                SubMenu =
                                {
                                    new ConfigTreeNode
                                    {
                                        Key = @"AdvancedPowerManagementConfiguration",
                                        Annotation = @"Advanced Power Management Configuration",
                                        Properties =
                                        {
                                            pType = "Int",
                                            Options = { "270", "300" },
                                            Default = "270",
                                            CurrentValue = "270"
                                        },
                                        RawDataMap =
                                        {
                                            Offset = "0x100",
                                            Size = "0x01",
                                            Value = "0x01"
                                        },
                                        SubMenu =
                                        {
                                            new ConfigTreeNode
                                            {
                                                Key = "IIOConfiguration",
                                                Annotation = "IIOConfiguration",
                                                RawDataMap =
                                                {
                                                    Offset = "0x300",
                                                    Size = "0x03",
                                                    Value = "0x03"
                                                },
                                                SubMenu =
                                                {
                                                    new ConfigTreeNode
                                                    {
                                                        Key = "IOATConfiguration",
                                                        Annotation = "IOAT Configuration",
                                                        RawDataMap =
                                                        {
                                                            Offset = "0x300",
                                                            Size = "0x03",
                                                            Value = "0x03"
                                                        },
                                                        SubMenu =
                                                        {                                           {
                                                            new ConfigTreeNode
                                                            {
                                                                Key = "DisableTPH",
                                                                Annotation = "Disable TPH",
                                                                RawDataMap =
                                                                {
                                                                    Offset = "0x300",
                                                                    Size = "0x03",
                                                                    Value = "0x03"
                                                                },
                                                                Observer =
                                                                {
                                                                    "Root.Edk2Manu.Socket",
                                                                    "Root.BootManagerMenu",
                                                                    "Root.Edk2Manu.Socket.AdvancedPowerManagementConfiguration"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        }
                    },
                    new ConfigTreeNode{
                        Key = @"BootManagerMenu",
                        Annotation = @"Boot Manage rMenu"
                    },
                    new ConfigTreeNode{
                        Key = @"BootMaintenanceManager",
                        Annotation = @"Boot Maintenance Manager"
                    },
                    new ConfigTreeNode{
                        Key = @"Continue",
                        Annotation = @"Continue"
                    },
                    new ConfigTreeNode{
                        Key = @"Reset",
                        Annotation = @"Reset"
                    },
                }
            };
            var json = new JsonFormatter(JsonConvert.SerializeObject(model)).Format();
            var jsonFile = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\EdkIIConfig.json";
            //File.WriteAllText(jsonFile, json);

            SmartConfigDataModel root = null;
            using (StreamReader file = File.OpenText(jsonFile))
            {
                var serializer = new JsonSerializer();
                try
                {
                    root = (SmartConfigDataModel)serializer.Deserialize(file, typeof(SmartConfigDataModel));
                    root.BuildTree();
                    string[] keys = { "Edk2Manu",
                                      "Socket",
                                      "AdvancedPowerManagementConfiguration",
                                      "IIOConfiguration",
                                      "IOATConfiguration",
                                      "DisableTPH" };
                    var jpath = "Root";
                    foreach (var key in keys)
                    {
                        jpath = string.Format("{0}.{1}", jpath, key);
                        var x = root.GetConfigNodeByJPath(jpath);
                    }

                    var updateDictFile = @"C:/mcu-release-automation/ReleaseAutomation/buildingblocks/SmartJsonConfig/unittest_smart_json_config/UpdateDictionary.json";
                }
                catch(Exception ex)
                {
                }
            }
            var updateMatrix = new UpdateMatrix
            {
                UpdateData =
                {
                    new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("0")),
                    new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("1"){
                        Properties = new Props{
                            pType = "Hex",
                            Options = {"0xe0", "0xe1", "0xe2", "0xe3" },
                            Default = "0xe3",
                            CurrentValue = "0xe3"
                        },
                        RawDataMap = new RawData{
                            Offset = "0x200",
                            Size ="0x02",
                            Value ="0x02"
                        }
                    }),
                    new KeyValuePair<string, UpdateMatrixData>(@"DisableTPH", new UpdateMatrixData("2")),
                }
            };
            json = new JsonFormatter(JsonConvert.SerializeObject(updateMatrix)).Format();
            jsonFile = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\UpdateDictionary.json";
            //File.WriteAllText(jsonFile, json);
            var jp = "Root.Edk2Manu.Socket.AdvancedPowerManagementConfiguration.IIOConfiguration.IOATConfiguration.DisableTPH";
            var instance = root.GetConfigNodeByJPath(jp);
            instance.SyncUpdate(instance);

        }
    }
}
