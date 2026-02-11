using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CreateShortCut
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR
    }

    public static class LoggingUtility
    {
        public static void LogDebug(string message)
        {
#if DEBUG
            WriteLog(LogLevel.DEBUG, message, null);
#endif
        }

        public static void LogInfo(string message)
        {
            WriteLog(LogLevel.INFO, message, null);
        }

        public static void LogWarn(string message)
        {
            WriteLog(LogLevel.WARN, message, null);
        }

        public static void LogError(string message)
        {
            WriteLog(LogLevel.ERROR, message, null);
        }

        public static void LogError(string message, Exception ex)
        {
            WriteLog(LogLevel.ERROR, message, ex);
        }

        private static void WriteLog(LogLevel level, string message, Exception ex)
        {
            try
            {
                string logMessage = FormatLogMessage(level, message, ex);

                // 複数のパスを試す
                string[] logPaths = {
                    Path.Combine(Application.StartupPath, "error.log"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CreateShortCut_error.log"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CreateShortCut", "error.log"),
                    Path.Combine(Path.GetTempPath(), "CreateShortCut_error.log")
                };

                bool logWritten = false;
                foreach (string logPath in logPaths)
                {
                    if (TryWriteLog(logPath, logMessage))
                    {
                        logWritten = true;
                        break;
                    }
                }

                // ログが書けなかった場合のデバッグ情報表示
                if (!logWritten)
                {
                    MessageBox.Show($"ログファイルへの書き込みに失敗しました。\n\nエラー情報:\n{message}\n\n試したパス:\n{string.Join("\n", logPaths)}", "ログ書き込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
                // 最終的なフォールバック
            }
        }

        private static bool TryWriteLog(string logPath, string logMessage)
        {
            try
            {
                // ディレクトリが存在しない場合は作成する
                string directory = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(logPath, logMessage, Encoding.GetEncoding("Shift_JIS"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string FormatLogMessage(LogLevel level, string message, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}");

            if (ex != null)
            {
                sb.AppendLine($"Exception Type: {ex.GetType().FullName}");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    sb.AppendLine($"Inner Exception: {ex.InnerException.GetType().FullName}");
                    sb.AppendLine($"Inner Message: {ex.InnerException.Message}");
                    sb.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }

                sb.AppendLine($"Source: {ex.Source}");
                sb.AppendLine($"Target Site: {ex.TargetSite}");
                sb.AppendLine($"Help Link: {ex.HelpLink}");
                sb.AppendLine($"Data: {string.Join(", ", ex.Data.Cast<System.Collections.DictionaryEntry>().Select(kvp => $"{kvp.Key}={kvp.Value}"))}");

                sb.AppendLine($"Application Path: {Application.StartupPath}");
                sb.AppendLine($"Working Directory: {Directory.GetCurrentDirectory()}");
                sb.AppendLine($"User: {Environment.UserName}");
                sb.AppendLine($"Machine: {Environment.MachineName}");
                sb.AppendLine($"OS Version: {Environment.OSVersion}");
                sb.AppendLine($"CLR Version: {Environment.Version}");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
