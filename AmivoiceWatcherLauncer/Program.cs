using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AmivoiceWatcherLauncher
{
    class Program
    {
        private const string file2launch = "AmivoiceWatcher.exe";
        [STAThread]
        static void Main(string[] args)
        {

            var pwd = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var executableFile = Path.Combine(pwd, file2launch);
            
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = executableFile;

            //~ if run trhough cmd.exe
            //psi.FileName = @"cmd";
            //psi.Arguments = "/C start \""+ executableFile +"\"";

            //psi.WindowStyle = ProcessWindowStyle.Normal;
            //psi.WorkingDirectory = ".";

            psi.WorkingDirectory = pwd;

            //Thread.Sleep(5000);
            Process.Start(psi);

            //~ Other methods
            //System.Diagnostics.Process.Start(executableFile);
            //Process.Start(executableFile);

            //Process.WaitForExit();
            //Thread.Sleep(15000);
            
        }
    }
}
