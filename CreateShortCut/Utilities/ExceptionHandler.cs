using System;
using System.IO;
using System.Windows.Forms;

namespace CreateShortCut.Utilities
{
    public static class ExceptionHandler
    {
        public static void HandleException(Exception ex, string context)
        {
            LoggingUtility.LogError($"{context}: {ex.Message}", ex);
            MessageBox.Show($"{context}:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void HandleAccessDenied(UnauthorizedAccessException ex, string context)
        {
            LoggingUtility.LogError($"アクセス拒否エラー ({context}): {ex.Message}", ex);
            MessageBox.Show($"アクセス拒否: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void HandleIOException(IOException ex, string context)
        {
            LoggingUtility.LogError($"IO エラー ({context}): {ex.Message}", ex);
            MessageBox.Show($"IO エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
