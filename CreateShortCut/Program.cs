using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateShortCut
{
    internal static class Program
    {
        private static readonly string MutexName = "CreateShortCutSingleInstance";

        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(true, MutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    // 既に実行中の場合は終了
                    MessageBox.Show("CreateShortCutは既に実行中です。", "重複実行", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
