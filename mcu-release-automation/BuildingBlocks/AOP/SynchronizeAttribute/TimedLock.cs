using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace McAfeeLabs.Engineering.Automation.AOP
{
#if DEBUG
    public class TimedLock : IDisposable
#else
    /// <summary>
    /// Using TimedLock to avoid the dead lock, ref IanG's blog http://www.interact-sw.co.uk/iangblog/2004/03/23/locking
    /// </summary>
    public struct TimedLock : IDisposable
#endif
    {
        private object _target;
        public static TimedLock Lock(object o, int seconds)
        {
            return Lock(o, TimeSpan.FromSeconds(seconds));
        }

        public static TimedLock Lock(object o, TimeSpan timeout)
        {
            TimedLock tl = new TimedLock(o);
            if (!Monitor.TryEnter(o, timeout))
            {
#if DEBUG
                System.GC.SuppressFinalize(tl);
#endif
                throw new LockTimeoutException();
            }

            return tl;
        }

        private TimedLock(object o)
        {
            _target = o;
        }

        public void Dispose()
        {
            Monitor.Exit(_target);

            // It's a bad error if someone forgets to call Dispose,
            // so in Debug builds, we put a finalizer in to detect
            // the error. If Dispose is called, we suppress the
            // finalizer.
#if DEBUG
            GC.SuppressFinalize(this);
#endif
        }

#if DEBUG
        ~TimedLock()
        {
            // If this finalizer runs, someone somewhere failed to
            // call Dispose, which means we've failed to leave
            // a monitor!
            System.Diagnostics.Debug.Fail("Undisposed lock");
        }
#endif
    }
    public class LockTimeoutException : Exception
    {
        public LockTimeoutException()
            : base("Timeout waiting for lock")
        {
        }
    }
}
