// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [Serializable]
    public class FileDetails
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public string FileNameWithoutPath { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public StatusType Status { get; set; }
        public DateTime Date { get; set; }
        public string Runner { get; set; }
        public ulong SubmissionID { get; set; }
        public string ErrorReasons { get; set; }

        public override string ToString()
        {
            return string.Format("ID={0}, Hash={1}, Filename={2}, Path={3}, Size={4}, Status={5}, Date={6}, Runner={7}, SubmissionID={8}, ErrorReasons={9}",
                ID, Hash, FileNameWithoutPath, FilePath, FileSize, Status, Date, Runner, SubmissionID, ErrorReasons);
        }

    }
}
