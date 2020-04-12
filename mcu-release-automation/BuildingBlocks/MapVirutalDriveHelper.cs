using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

using System.Runtime.InteropServices;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public partial class MapVirutalDriveHelper : IDisposable
    {
        #region Declarations

        private static MapVirutalDriveHelper _instance;
        private static object _syncObj = new object();
        private Dictionary<string, string> _mapDictionary = new Dictionary<string, string>();

        public const int DDD_RAW_TARGET_PATH = 0x00000001;
        public const int DDD_REMOVE_DEFINITION = 0x00000002;
        public const int DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
        public const int DDD_NO_BROADCAST_SYSTEM = 0x00000008;

        #endregion

        #region Constructor

        private MapVirutalDriveHelper()
        {
        }

        #endregion

        #region Properties

        public static MapVirutalDriveHelper Instance
        {
            get
            {
                lock (_syncObj)
                {
                    if (null == _instance)
                        _instance = new MapVirutalDriveHelper();
                    return _instance;
                }
            }
        }

        #endregion

        #region Public Methods

        public void ResetAllVirtualDrives()
        {
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                if (driveInfo.DriveType == DriveType.Fixed)
                //try
                //{
                //    var drive = driveInfo.Name.Replace(@":\", string.Empty);
                //    UnmapDrive(drive[0], AppDomain.CurrentDomain.BaseDirectory);
                //    Process.Start(string.Format(@"subst {0}: /d", drive));
                //}
                //catch { }
                    ResetVirtualDrive(driveInfo.Name);
            }
        }

        private static void ResetVirtualDrive(string drive)
        {
            try
            {
                var driveName = drive.Replace(@":\", string.Empty);
                UnmapDrive(driveName[0], AppDomain.CurrentDomain.BaseDirectory);
                Process.Start(string.Format(@"subst {0}: /d", driveName));
            }
            catch { }
        }

        public string MapDrive( string localFolder,
                                List<string> exclusions,
                                bool bubbleException = false)
        {
            string results = string.Empty;

            if (null != exclusions)
            {
                if (_mapDictionary.ContainsKey(localFolder))
                    _mapDictionary.Remove(localFolder);
                exclusions.ForEach(x => ResetVirtualDrive(x));
            }

            if (Directory.Exists(localFolder))
            {
                if (_mapDictionary.ContainsKey(localFolder))
                    results = _mapDictionary[localFolder];
                else
                {
                    var exclusionList = new List<string>();
                    var success = false;
                    for (int i = 0; i < 20 && !success; ++i)
                    {
                        var nextDrive = GetNextDriveLetter(exclusionList);
                        try
                        {
                            if (DefineDosDevice(0, devName(nextDrive[0]), localFolder))
                            {
                                results = string.Format(@"{0}:\", nextDrive[0]);
                                _mapDictionary.Add(localFolder, results);
                                if (Directory.Exists(results))
                                {
                                    var tempDir = Path.Combine(results, @"Temp");
                                    Directory.CreateDirectory(tempDir);
                                    if (Directory.Exists(tempDir)) Directory.Delete(tempDir);

                                    success = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        if (!success)
                            exclusionList.Add(nextDrive);
                    }
                    if (bubbleException && string.IsNullOrEmpty(results))
                        throw new ApplicationException(string.Format(@"--- Failed to map local folder <{0}> to a drive", localFolder));
                }
            }

            return results;
        }

        public void Dispose()
        {
            var itr = _mapDictionary.GetEnumerator();
            while (itr.MoveNext())
            {
                UnmapDrive(itr.Current.Value[0], itr.Current.Key);
            }
            _mapDictionary.Clear();
        }

        public string SubstDrive(string localFolder)
        {
            string results = string.Empty;
            if (Directory.Exists(localFolder))
            {
                localFolder.TryDemandFullAccess();
                if (_mapDictionary.ContainsKey(localFolder))
                    results = _mapDictionary[localFolder];
                else
                {
                    var exclusionList = new List<string>();
                    var success = false;

                    for (int i = 0; i < 20 && !success; ++i)
                    {
                        var nextDrive = GetNextDriveLetter(exclusionList);
                        if (exclusionList.Contains(nextDrive)) continue;
                        try
                        {
                            Process.Start(string.Format(@"subst {0}: \\{1}\{2}", nextDrive[0], Environment.MachineName, localFolder.Replace(@":", @"$")));
                            results = string.Format(@"{0}:\", nextDrive[0]);
                            _mapDictionary.Add(localFolder, results);
                            if (!Directory.Exists(results))
                                success = true;
                        }
                        catch (Exception ex)
                        {
                        }
                        if (!success)
                            exclusionList.Add(nextDrive);
                    }
                    if (string.IsNullOrEmpty(results))
                        throw new ApplicationException(string.Format(@"--- Failed to subst local folder <{0}> to a drive", localFolder));
                }
            }

            return results;
        }


        #endregion

        #region Private Methods

        private void DisposeSubst()
        {
            var itr = _mapDictionary.GetEnumerator();
            while (itr.MoveNext())
            {
                UnmapDrive(itr.Current.Value[0], itr.Current.Key);
                try
                {
                    Process.Start(string.Format(@"subst.exe {0}: /d", itr.Current.Value[0]));
                }
                catch { }
            }
            _mapDictionary.Clear();
        }

        private static void UnmapDrive(char letter, string path)
        {
            if (!DefineDosDevice(DDD_REMOVE_DEFINITION | DDD_EXACT_MATCH_ON_REMOVE, devName(letter), path))
                throw new Win32Exception();
        }

        private static string GetDriveMapping(char letter)
        {
            var sb = new StringBuilder(259);
            if (QueryDosDevice(devName(letter), sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2)
                    return string.Empty;
                throw new Win32Exception();
            }
            return sb.ToString().Substring(4);
        }

        private static string GetNextDriveLetter(List<string> exclusionList = null)
        {
            var inUseList = new List<string>();
            if (null != exclusionList && exclusionList.Count > 0)
                inUseList.AddRange(exclusionList);

            var drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
                inUseList.Add(drive.Name.Substring(0, 1).ToUpper());

            char[] alphas = "EFGHIJKLMNOPQRSTUVWXY".ToCharArray();

            foreach (char alpha in alphas)
                if (!inUseList.Contains(alpha.ToString())) return alpha.ToString();

            return null;
        }

        private static string devName(char letter)
        {
            return new string(char.ToUpper(letter), 1) + ":";
        }

        #endregion

        #region Dll import

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string devname, string path);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string devname, StringBuilder buffer, int bufSize);

        #endregion
    }
}
