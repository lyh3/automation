using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using Microsoft.Practices.Unity;
using SevenZip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class SevenZipSharpWrapper : IZipProvider
    {
        #region Declarations

        private string _sourceArchiveFile;
        private bool _isArchiveFile;
        private string _password;
        private InArchiveFormat? _inputArchiveFormat;

        #endregion

        #region Constructor

        [InjectionConstructor]
        public SevenZipSharpWrapper()
        {
            Initialization();
        }

        public SevenZipSharpWrapper(string sourceFile, string password = null)
        {
            Initialization();
            SourcArchive = sourceFile;
            _password = password;
        }

        #endregion

        #region Properties

        public bool IsArchive { get { return _isArchiveFile; } }
        public InArchiveFormat? InputArchiveFormat { get { return _inputArchiveFormat; } }
        public string LastErrorMessage { get; set; }
        public bool? ThrowException { get; set; }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string SourcArchive
        {
            get { return _sourceArchiveFile; }
            set
            {
                _sourceArchiveFile = value;
                _isArchiveFile = false;
                try
                {
                    using (var extractor = new SevenZipExtractor(_sourceArchiveFile))
                    {
                        _inputArchiveFormat = extractor.Format;
                        _isArchiveFile = null != _inputArchiveFormat
                                        && (int)_inputArchiveFormat.Value >= (int)InArchiveFormat.SevenZip
                                        && (int)_inputArchiveFormat.Value <= (int)InArchiveFormat.Vhd;
                    }
                }
                catch { }
            }
        }

        #endregion


        #region Public Methods

        public void ExtractArchiveTo(string distinationFolder, string password = null)
        {
            LastErrorMessage = string.Empty;
            Exception exception = null;

            if (!_isArchiveFile) return;
            _password = password;
            try
            {
                using (var extractor = ExtractorFactory())
                {
                    try
                    {
                        extractor.FileExtractionStarted += (s, e) => { Trace.WriteLine(String.Format(@"--- Extracting archive file <{0}> [{1}%]{2}", e.FileInfo.FileName, e.PercentDone, Environment.NewLine)); };
                        extractor.FileExists += (o, e) => { Trace.WriteLine(string.Format(@"File <{0} already exists and will be overwritted.{1}", e.FileName, Environment.NewLine)); };
                        extractor.ExtractionFinished += (s, e) => { Trace.WriteLine(string.Format(@"--- Extracting archive <{0}> complete.{1}", _sourceArchiveFile, Environment.NewLine)); };
                        extractor.ExtractArchive(distinationFolder);
                    }
                    catch (ExtractionFailedException extractionFailure)
                    {
                        exception = extractionFailure;
                        LastErrorMessage = string.Format(@"--- Failed to extract archive <{0}> to <{1}>{2}", _sourceArchiveFile, distinationFolder, Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        LastErrorMessage = ex.Message;
                    }
                };
            }
            catch { }

            if (null != exception && null != ThrowException && ThrowException.Value) throw exception;
        }
         
        private SevenZipExtractor ExtractorFactory()
        {
            return string.IsNullOrEmpty(_password)
                          ? new SevenZipExtractor(_sourceArchiveFile)
                          : new SevenZipExtractor(_sourceArchiveFile, _password);
        }

        private dynamic ExtractFropStream(Stream fileStream)
        {
            var results = new List<dynamic>();
            if (null != fileStream && fileStream.CanRead)
            {
                using (var extractor = new SevenZipExtractor(fileStream))
                {
                    foreach (string file in extractor.ArchiveFileNames)
                    {
                        if (string.IsNullOrEmpty(file.ResolveSingleFilePath()))
                            continue;
                        var memoryStream = new MemoryStream();
                        extractor.ExtractFile(file, memoryStream);
                        results.Add(memoryStream.GetBuffer());
                    }
                }
            }
            return results;
        }

        #endregion

        #region Private Methods

        private void Initialization()
        {
            const string sevenZipDllName = @"7z.dll";
            var sevenzipInstallPath = string.Empty;

            _isArchiveFile = false;

            try
            {
                var sevenzipInstallRegKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\7-Zip";
                sevenzipInstallPath = sevenzipInstallRegKey.GetValue(@"Path").ToString();
                if (!File.Exists(sevenzipInstallPath))
                    sevenzipInstallPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch { }

            SevenZipExtractor.SetLibraryPath(Path.Combine(sevenzipInstallPath, sevenZipDllName));
        }

        #endregion
    }
}
