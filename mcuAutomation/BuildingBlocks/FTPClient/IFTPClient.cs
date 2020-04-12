using System;
using System.Collections.Generic;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public interface IFTPClient
    {
        string LastErrorMessage { get; }
        string FtpUri { get; }
        string Host { get; }
        string UserName { get; }
        string PassWord { get; }
        int? TimeOut { get; set; }
        bool IsBubbleUpException { get; set; }
        bool Exists { get; }
        bool PassiveMode { get; set; }
        string GetParentDirectory();
        void Initialize(string uri);
        bool Ping();
    }

    public interface IFTPFile : IFTPClient
    {
        int ChunkSize { get; set; }
        string FileName { get; }
        bool UploadFile(string localPath, int retry = 0);
        bool UploadFile(byte[] content, bool append = false, int retry = 0);
        bool UploadStream(Stream stream, int retry = 0);
        byte[] DownloadFile(int retry = 0);
        bool DownloadFile(string localPath, int retry = 0);
        bool DeleteFile(int retry = 0);
        long GetFileSize(int retry = 0);
    }

    public interface IFTPFolder : IFTPClient
    {
        string FolderName { get; }
        bool CreateSubFolder(string folderName, int retry = 0);
        bool DeleteSubFolder(string folderName);
        bool ListDirectory(out List<String> folderList, out List<String> fileList, string folderName = "");
    }
}
