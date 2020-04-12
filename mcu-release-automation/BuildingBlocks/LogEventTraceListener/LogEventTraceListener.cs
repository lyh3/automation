using System;
using System.Text;
using System.Diagnostics;

namespace McAfeeLabs.Engineering.Automation.Base.LogEventTraceListener
{
    public class LogEventTraceListener : TraceListener
    {
        #region Declarations

        private EventLog _eventLog;
        private long MaxAllowedKilloByte = 40000000000; //4 GB

        #endregion

        #region Constructor

        public LogEventTraceListener(string logName, 
                                     string source, 
                                     string machineName,
                                     long? maxKiloByte,
                                     int ? retentionDays)
        {
            _eventLog = new EventLog();

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, logName);

            _eventLog.Source = source;
            _eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, retentionDays: null == retentionDays ? 30 : retentionDays.Value);

            _eventLog.MaximumKilobytes = null == maxKiloByte || maxKiloByte.Value <= 0 ? 256 : 64 * (int)(maxKiloByte.Value / 64);
            if (_eventLog.MaximumKilobytes > MaxAllowedKilloByte)
                _eventLog.MaximumKilobytes = MaxAllowedKilloByte;
            _eventLog.MachineName = machineName;
        }

        #endregion

        #region Public Methods

        static public LogEventTraceListener AddLogEventTraceListener( object client, 
                                                                      string logName,
                                                                      string machineName,
                                                                      long? maxKiloByte,
                                                                      int? retentionDays)
        {
            if (null == client) return null;
            return AddListner(logName,
                              client.GetType().Name,
                              machineName,
                              maxKiloByte,
                              retentionDays);
        }

        public override void Write(string message)
        {
            this.WriteLine(message);
        }

        override public void WriteLine(string message)
        {
            _eventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        #endregion

        #region Private Methods

        private static LogEventTraceListener AddListner(string logName,
                                                        string source,
                                                        string machineName,
                                                        long? maxKiloByte,
                                                        int? retentionDays)
        {
            var fileLogListener = new LogEventTraceListener(logName, 
                                                            source,
                                                            machineName, 
                                                            maxKiloByte, 
                                                            retentionDays);
            Trace.Listeners.Add(fileLogListener);
            return fileLogListener;
        }

        #endregion
    }
}
