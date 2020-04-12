using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using NUnit.Framework;
using IntelDCGSpsWebService.Models;
using IntelDCGSpsWebService.Models.Buildingblocks;

namespace DcPaeWebServiceUnitest
{
    [TestFixture]
    public class ConsoleCallerTest : IConsoleCaller
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32", SetLastError = true)]
        static extern bool FreeConsole();
        [DllImport("kernel32", SetLastError = true)]
        static extern bool AllocConsole();
        public event ConsoleOutputCallback consoleCallbackDelegate;

        [Test]
        public void AttachConsoleTest()
        {
            //AllocConsole(();
            //Console.WriteLine("This is line one.");
            //Console.WriteLine("this is my second line.");
            //Console.WriteLine("This is line 3.");
            //string myString = Console.ReadLine();
            //FreeConsole();
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process p = Process.Start(psi);

            StreamWriter sw = p.StandardInput;
            StreamReader sr = p.StandardOutput;

            sw.WriteLine("Hello world!");
            sr.Close();
        }
        [Test]
        public void TestConsoleCaller()
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = @"d:\IpmiTool-1.8.11.i4-win";
            process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

            //process.StartInfo.FileName = Path.Combine(@"d:\IpmiTool-1.8.11.i4-win", "ipmitool.exe");
            //process.StartInfo.Arguments = " -I lanplus -H 10.23.60.3 -U root -P superuser -b 6 -t 44 raw 6 1";

            using (ProcessLauncher launcher = new ProcessLauncher(this, process))
            {
                launcher.consoleOutlineEvent += this.onConsoleOutputDataReceived;
                launcher.consoleErrorEvent += this.onConsoleErrortDataReceived;
                try
                {
                    launcher.Start();
                    //launcher.SendInput(string.Format(" -I lanplus -H 10.23.60.3 -U root -P superuser -b 6 -t 44 raw 6 1{0}", Convert.ToChar(0x0d)));
                    //launcher.SendInput(string.Format("d:{0}", Convert.ToChar(0x0d)));
                    //launcher.SendInput(string.Format(@"cd d:\IpmiTool-1.8.11.i4-win{0}", Convert.ToChar(0x0d)));
                    //launcher.SendInput(string.Format("dir{0}", Convert.ToChar(0x0d)));
                }
                finally
                {
                    //launcher.SendInput("exit()");
                }
            }
        }
        [Test]
        public void TestConsoleCallerPython()
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = @"C:\";
            process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            process.StartInfo.Arguments = @"/c ""C:\python27\python.exe -i""";
            using (ProcessLauncher launcher = new ProcessLauncher(this, process))
            {
                launcher.consoleOutlineEvent += this.onConsoleOutputDataReceived;
                launcher.consoleErrorEvent += this.onConsoleErrortDataReceived;
                try
                {
                    launcher.Start();
                    launcher.SendInput("import sys;\n");
                    launcher.SendInput("print \"Test.\";\n");
                }
                finally
                {
                    launcher.SendInput("exit()");
                }
            }
        }

        public void onConsoleOutputDataReceived(object sender, DataReceivedEventArgs args)
        {

        }

        public void onConsoleErrortDataReceived(object sender, DataReceivedEventArgs args)
        {

        }

    }
}
