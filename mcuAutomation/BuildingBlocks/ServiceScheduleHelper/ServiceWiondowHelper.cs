using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Configuration;

using McAfeeLabs.Engineering.Automation.Base;

namespace McAfeeLabs.Engineering.Automation.Base.ServiceScheduleHelper
{
    /// <summary>
    /// Summary description for ServiceWiondowHelper.
	/// </summary>
	[Serializable]
	public class ServiceWiondowHelper
	{
		private ServiceWindow _serviceWindow = null;
		private Hashtable _serviceWindowTable_Week = new Hashtable();
		private bool _serviceWindowSpecified = false;

		public ServiceWiondowHelper( XmlDocument xmlDocServiceWindow )
		{
			if ( null == xmlDocServiceWindow )
				throw new ArgumentException( string.Format( "--!--{0}:Input XmlDocument instance can not be null", this.ToString()), "xmlDocServiceWindow" );

            try
            {
                _serviceWindow = CommonUtility.XmlRetrieve<ServiceWindow>(xmlDocServiceWindow);

                if (null != _serviceWindow.Window)
                {
                    PopulateServiceWindowTable();
                    _serviceWindowSpecified = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("--!--{0}:ServiceWiondowHelper, exception caught : {1}", this.GetType().Name, ex.Message));
            }
        }

        public bool ServiceWindowSpecified
        {
            get { return _serviceWindowSpecified; }
            set { _serviceWindowSpecified = value; }
        }

        public bool IsInServiceWindow(out int delayPeriod, out ServiceWindowWindow sericeWindow)
		{
			bool bFallingAWindow = false;

			delayPeriod = 0;
            sericeWindow = null;

			if ( !_serviceWindowSpecified )
			{
				return false;
			}

			DateTime timeNow = DateTime.Now;
			
			if ( null == _serviceWindowTable_Week || _serviceWindowTable_Week.Count == 0 )
			{
				bFallingAWindow = false;
			}
			else
			{
				Hashtable serviceWindowTable_Day = null;
				if ( _serviceWindowTable_Week.ContainsKey( timeNow.DayOfWeek ) )
				{
					serviceWindowTable_Day = _serviceWindowTable_Week[ timeNow.DayOfWeek ] as Hashtable;
					foreach( string key in serviceWindowTable_Day.Keys )
					{
						sericeWindow = serviceWindowTable_Day[ key ] as ServiceWindowWindow;
						DateTime t_Start = DateTime.Parse( sericeWindow.StartTime );
						DateTime t_End = DateTime.Parse( sericeWindow.EndTime );

						if ( (timeNow > t_Start && timeNow > t_End)  || ( timeNow < t_Start && timeNow < t_End ) )
                        {
                            sericeWindow = null;
							continue;
						}
						else
                        if (timeNow >= t_Start && timeNow <= t_End)
                        {
                            bFallingAWindow = true;
                            delayPeriod = ((t_End.Hour - timeNow.Hour) * 60 + (t_End.Minute - timeNow.Minute)) * 60 + (t_End.Second - timeNow.Second);
                            break;
                        }
					}
				}
			}

			return bFallingAWindow;
		}

		#region Private Method

		private bool IsValidServiceWindow( ServiceWindowWindow window )
		{
			bool valid = false;

			try
			{
				DateTime t_start = DateTime.Parse( window.StartTime );
				DateTime t_end = DateTime.Parse( window.EndTime );

				if ( DateTime.Compare( t_start, t_end ) > 0 )
				{
					valid = false;
					System.Diagnostics.EventLog.WriteEntry(  "Invalid exclusive window setting ignored", string.Format( "The start time <{0}> can not be greater than the end time <{1}>for an exclusive service window settings", window.StartTime, window.EndTime ) );
				}
				else
				{
					valid = true;
				}
			}
			catch
			{
				System.Diagnostics.EventLog.WriteEntry(  "Invalid exclusive window setting ignored", string.Format( "Either the start time <{0}> or the end time <{1}> for an exclusive service window settings is invalid", window.StartTime, window.EndTime ) );
			}

			return valid;
		}

		private void PopulateServiceWindowTable()
		{
			if ( null != _serviceWindow )
			{
				Hashtable serviceWindowTable_Day = null;
				ArrayList keyList = new ArrayList();

				try
				{
					foreach( ServiceWindowWindow window in _serviceWindow.Window )
					{
						if ( !IsValidServiceWindow( window ) )
						{
							continue;
						}

						try
						{
							DayOfWeek day = ConvertStringToDayOfWeek( window.Day );

							if ( !_serviceWindowTable_Week.ContainsKey( day ) )//day is the weeke hashtable key
							{
								serviceWindowTable_Day = new Hashtable();
								_serviceWindowTable_Week.Add( day, serviceWindowTable_Day );

							}

							if ( _serviceWindowTable_Week.ContainsKey( day ) )
							{
								serviceWindowTable_Day = (Hashtable)_serviceWindowTable_Week[ day ];
							}

							Hashtable _tempTable = new Hashtable();
							_tempTable = (Hashtable)serviceWindowTable_Day.Clone();
							serviceWindowTable_Day.Clear();
							ArrayList _keyList = new ArrayList();
							_keyList = keyList.Clone() as ArrayList;
							keyList.Clear();

							if ( _tempTable.Count > 0 )
							{
								DateTime tNewStart = DateTime.Parse( window.StartTime );
								bool bInserted = false;

								for ( int i = 0; i < _tempTable.Count; ++i )
								{
									ServiceWindowWindow winExisting = _tempTable[ _keyList[i] ] as ServiceWindowWindow;
									DateTime tStart = DateTime.Parse( winExisting.StartTime );
									if ( DateTime.Compare( tStart, tNewStart ) <= 0 )//sort the exclusive windows accendtly
									{
										if ( !serviceWindowTable_Day.ContainsKey( winExisting.StartTime ) )
										{
											serviceWindowTable_Day.Add( winExisting.StartTime, winExisting );
											keyList.Add( winExisting.StartTime );
										}
									}
									else
									{
										if ( !serviceWindowTable_Day.ContainsKey( window.StartTime ) )
										{
											serviceWindowTable_Day.Add( window.StartTime, window );//start time is key of day hashtable
											keyList.Add( window.StartTime );
											bInserted = true;
										}
									}
								}

								if ( !bInserted )
								{
									if ( !serviceWindowTable_Day.ContainsKey( window.StartTime ) )
									{
										serviceWindowTable_Day.Add( window.StartTime, window );
										keyList.Add( window.StartTime );
										bInserted = true;
									}
								}
							}
							else
							{
								if ( !serviceWindowTable_Day.ContainsKey( window.StartTime ) )
								{
									serviceWindowTable_Day.Add( window.StartTime, window );
									keyList.Add( window.StartTime );
								}
							}
						}
						catch( Exception ex )
						{
							System.Diagnostics.EventLog.WriteEntry(  "Invalid exclusive window setting ignored", ex.Message );
						}
					}
			
					if ( _serviceWindowTable_Week.Count > 0 )
					{
						_serviceWindowSpecified = true;
					}
				}
				catch( Exception ex )
				{
					Debug.Write( ex.Message );
					throw ex;
				}
			}
		}

		private ServiceWindowWindow FindFirstAvailableServiceWindowInADay( Hashtable serviceWindowTable_Day )
		{
			ServiceWindowWindow w = null;

			foreach( string key in serviceWindowTable_Day.Keys )
			{
				w = serviceWindowTable_Day[ key ] as ServiceWindowWindow;
				break;
			}

			return w;
		}

		private DayOfWeek ConvertStringToDayOfWeek( string day )
		{
			DayOfWeek retVal = DayOfWeek.Monday;

			if ( null == day || day == string.Empty )
			{
				throw new ArgumentException( "The value for day can not be null or empty", "day" );
			}

			string _day = day.Trim().ToUpper();
			
			switch( _day )
			{
				case "M":
				case "MO":
				case "MONDAY":
					retVal = DayOfWeek.Monday;
					break;

				case "T":
				case "TU":
				case "TUESDAY":
					 retVal = DayOfWeek.Tuesday;
					break;

				case "W":
				case "WEDNESDAY":
					retVal = DayOfWeek.Wednesday;
					break;

				case "TH":
				case "THURSDAY":
					retVal = DayOfWeek.Thursday;
					break;

				case "FR":
				case "FRIDAY":
					retVal = DayOfWeek.Friday;
					break;

				case "SA":
				case "SATURDAY":
					retVal = DayOfWeek.Saturday;
					break;

				case "SUN":
				case "SU":
				case "SUNDAY":
					retVal = DayOfWeek.Sunday;
					break;

				default:
					throw new ArgumentException( "Invalid day specified.", day );
					break;
			}

			return retVal;
		}

		#endregion
	}
}
