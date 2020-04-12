using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    public class FolderInspectorTest : TestHarness
    {
        [Test]
        public void FolderInspecterCopyReceiverTest()
        {
            var sourceFolders = new List<string>();
            sourceFolders.Add(@"G:\1");
            var folderInspecterCopyReceiver = new FolderInspecterCopyReceiver( sourceFolders,
                                                                               2,
                                                                               @".\Receive");
            folderInspecterCopyReceiver.Inspect();
        }

        [Test]
        public void FilteredFolderInspectReceiverTest()
        {
            var filterList = new List<string>(new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AOP.Library.dll"),
                                                      Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DataAccessLayer.dll"), 
                                                      Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Libraries.dll") });
            const int returnCount = 25;
            const int maxSize = 1000000;
            var filteredFolderInspectorReceiver = new FilteredFolderInspectorReceiver( new[] { AppDomain.CurrentDomain.BaseDirectory },
                                                                                       batchSize:1,
                                                                                       recursive:false,
                                                                                       filterList:filterList,
                                                                                       returnCount:returnCount,
                                                                                       maxSize:maxSize);//1000 KB
            filteredFolderInspectorReceiver.BatchResultsForward += (s, e) =>
            {
                Assert.AreEqual(returnCount, filteredFolderInspectorReceiver.ReceivedFiles.Count );
                Assert.AreEqual(3, filteredFolderInspectorReceiver.FileExclusionsYield.Count);
                filteredFolderInspectorReceiver.FileExclusionsYield.ForEach(x => Assert.IsTrue(x is FileDetails));
                filteredFolderInspectorReceiver.ReceivedFiles.ForEach(x =>
                {
                    Assert.IsTrue(null == filterList.FirstOrDefault(f => f.CompareTo(x) == 0));
                    var fileInfo = new FileInfo(x);
                    Assert.IsTrue(fileInfo.Length < maxSize);
                });
            };  

            filteredFolderInspectorReceiver.Inspect();
        }
    }
}
