using System;
using System.Runtime.InteropServices;

using log4net;

namespace McAfee.Service.DataModel
{
    abstract public class MockWorkerState : WorkerState
    {
        #region Declarations

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private const int MF_BYCOMMAND = 0x00000000;
        protected static IntPtr _thisConsole;
        public const int SC_CLOSE = 0xF060;

        #endregion

        #region Constructor

        public MockWorkerState(ILog logger)
            : base(logger)
        {
        }

        #endregion

        #region Protected Methods

        public IntPtr ThisConsole
        {
            get { return _thisConsole; }
            set { _thisConsole = value; }
        }

        #endregion

        #region Win32

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;      
            public int Top;       
            public int Right;     
            public int Bottom;    
        }
        #endregion
    }
}
