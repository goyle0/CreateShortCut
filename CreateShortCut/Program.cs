using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateShortCut
{
    internal static class Program
    {
        private static bool isRunning = false; // アプリケーションが既に実行中かどうかを追跡するフラグを追加

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (isRunning)
            {
                return; // アプリケーションが既に実行中の場合、Main メソッドを終了する
            }

            isRunning = true; // アプリケーションが実行中であることを示すフラグを設定

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            isRunning = false; // アプリケーションが終了したらフラグをリセットする
        }
    }
}
