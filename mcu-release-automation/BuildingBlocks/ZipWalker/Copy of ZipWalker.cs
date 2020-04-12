using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ionic.Zip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class ZipWalker : ZipInspectorReceiver//FolderInspectorReceiver, IDisposable
    {
        #region Declarations

        protected IZipWalkerVisitor _visitor;
        //protected string _baseDir;
        //protected string _password;
        //protected string _lastErrorMessage;
        //protected Queue<string> _unpackedFilequeue = new Queue<string>();
        //protected Stack<string> _unpackedFolderstack = new Stack<string>();
        //protected Dictionary<string, int> _recuringdictionary = new Dictionary<string, int>();
        protected List<ZipWalker> _childList = new List<ZipWalker>();
        protected bool _canDispose = false;

        #endregion

        #region Constructors

        public ZipWalker(IEnumerable<string> sourceFolders,
                          int batchSize)
            : base(sourceFolders, batchSize, recursive: false)
        {
            _baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetType().Name);
            if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
            _unpackedFolderstack.Push(_baseDir);
            _password = @"infected";
        }

        public ZipWalker(IZipWalkerVisitor visitor,
                          IEnumerable<string> sourceFolders,
                          int batchSize)
            : this(sourceFolders, batchSize)
        {
            _visitor = visitor;
        }

        #endregion

        #region Properties

        //public bool? ThrowException { get; set; }
        //public string LastErrorMessage { get { return _lastErrorMessage; } }

        public bool CanDispose
        {
            get { return _canDispose; }
            set
            {
                _canDispose = value;
                _childList.ForEach(x => x.CanDispose = value);
            }
        }

        #endregion

        #region Public Methods

        override public void Dispose()
        {
            if (!CanDispose) return;

            //try
            //{
            //    while (_unpackedFilequeue.Count > 0)
            //    {
            //        var file = _unpackedFilequeue.Dequeue();
            //        if (file.RemoveFileReadOnlyAttribute() && File.Exists(file))
            //            File.Delete(file);
            //    }
            //    while (_unpackedFolderstack.Count > 0)
            //    {
            //        var folder = _unpackedFolderstack.Pop();
            //        if (Directory.Exists(folder))
            //            Directory.Delete(folder, true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (null != ThrowException && ThrowException.Value == true)
            //        throw ex;
            //}
            _childList.ForEach(x => x.Dispose());
            base.Dispose();
        }

        #endregion

        #region Private Methods

        override protected void OnFileInspectBatchResults(object sender, EventArgs e)
        {
            if (null != _visitor && _folderInspector.CurrentFiles.Count > 0)
                _visitor.Visit(_folderInspector.CurrentFiles);

            _folderInspector.CurrentFiles.ForEach(filepath =>
            {
                if (ZipFile.IsZipFile(filepath))
                {
                    var md5 = new FileInfo(filepath).MD5().ToString();
                    RetrieveZipEntryHeirachy(filepath);
                }
            });
        }

        //protected void RetrieveZipEntryHeirachy(string filename)
        //{
        //    for (; null != RecurseExtract(filename, string.Empty, _baseDir); ) ;
        //}

        //override protected IEnumerable<ZipEntry> RecurseExtract(string filepath, 
        //                                                        string subfolderPreffix,
        //                                                        string basdir = null,
        //                                                        int? deepLevel = null)
        //{
        //    Exception exception = null;
        //    //var TrimArray = new[] { '\\', '_', '/' };
        //    if (!string.IsNullOrEmpty(filepath))
        //    {
        //        var md5 = new FileInfo(filepath).MD5().ToString(); 
        //        try
        //        {
        //            using (var zipfile = ZipFile.Read(filepath))
        //            {
        //                zipfile.Password = _password;
        //                foreach (var entry in zipfile.Entries)
        //                {
        //                    var f = ExtractContents(filepath, 
        //                                            subfolderPreffix, 
        //                                            basdir, 
        //                                            md5, 
        //                                            entry);

        //                    if (ZipFile.IsZipFile(f))
        //                        return RecurseExtract(f, subfolderPreffix + md5);//"_");
        //                }
        //            }
        //        }
        //        catch (BadPasswordException bdpwdex)
        //        {
        //            _lastErrorMessage = string.Format(@"The zip file <{0}> is pass word protected, a correct pass word must be provided.", filepath);
        //            exception = bdpwdex;
        //        }
        //        catch (BadReadException rdex)
        //        {
        //            _lastErrorMessage = string.Format(@"Exception caught when read zip file <{0}>, error was:{1}.", filepath, null != rdex.InnerException ? rdex.InnerException.Message : rdex.Message);
        //            exception = rdex;
        //        }
        //        catch (Exception ex)
        //        {
        //            exception = ex;
        //        }

        //        if (null != exception)
        //        {
        //            Dispose();
        //            if (null != ThrowException && ThrowException.Value == true)
        //                throw exception;
        //        }
        //    }
        //    return null;
        //}

        override protected string ProcessEntry(string filepath,
                                                string subfolderPreffix,
                                                string basdir,
                                                string md5,
                                                ZipEntry entry)
        {
            string dir;
            string unpackedFilename;
            ExtractConten(filepath, subfolderPreffix, basdir, md5, entry, out dir, out unpackedFilename);

            var zipWalker = new ZipWalker(_visitor,
                                          new[] { dir },
                                          _folderInspector.BatchSize);
            zipWalker.Start();
            _childList.Add(zipWalker);

            return unpackedFilename;
        }

        //protected void ExtractConten( string filepath,
        //                              string subfolderPreffix,
        //                              string basdir,
        //                              string md5,
        //                              ZipEntry entry,
        //                              out string dir,
        //                              out string unpackedFilename)
        //{
        //    var filename = Path.GetFileName(filepath);

        //    var parentpath = !string.IsNullOrEmpty(basdir) ? _baseDir : filepath.Replace(filename, string.Empty).Trim(TrimArray);
        //    dir = string.Format(@"{0}\{1}{2}",
        //                            parentpath,
        //                            subfolderPreffix,
        //                            !string.IsNullOrEmpty(filename) ? filename : md5);

        //    CreateSubfolder(filename, dir);

        //    entry.Extract(dir, ExtractExistingFileAction.OverwriteSilently);
        //    unpackedFilename = string.Format(@"{0}\{1}", dir, entry.FileName);

        //    _unpackedFilequeue.Enqueue(unpackedFilename);
        //}

        //private void CreateSubfolder(string filename, string dir)
        //{
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //        if (!_recuringdictionary.ContainsKey(filename))
        //            _recuringdictionary.Add(filename, 1);
        //        else
        //        {
        //            int count;
        //            _recuringdictionary.TryGetValue(filename, out count);
        //            _recuringdictionary.Remove(filename);
        //            _recuringdictionary.Add(filename, count + 1);
        //        }
        //        _unpackedFolderstack.Push(dir);
        //    }
        //} 

        #endregion
    }
}
