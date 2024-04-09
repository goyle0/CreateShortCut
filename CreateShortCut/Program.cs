using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateShortCut
{
    internal static class Program
    {
        private static bool isRunning = false;

        [STAThread]
        static void Main()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;

            if (IsAlreadyRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            isRunning = false;
        }

        private static bool IsAlreadyRunning()
        {
            string currentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(currentProcessName);
            return processes.Length > 1;
        }
    }
}
