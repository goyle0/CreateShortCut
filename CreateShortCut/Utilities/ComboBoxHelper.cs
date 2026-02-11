using System;
using System.IO;
using System.Windows.Forms;

namespace CreateShortCut.Utilities
{
    public static class ComboBoxHelper
    {
        public static bool LoadDirectories(ComboBox comboBox, string path)
        {
            comboBox.Items.Clear();

            if (!Directory.Exists(path))
            {
                LoggingUtility.LogError($"指定されたフォルダパスが存在しません: {path}");
                MessageBox.Show($"指定されたフォルダパスが存在しません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!SecurityUtility.HasDirectoryAccess(path))
            {
                LoggingUtility.LogError($"フォルダパスへのアクセス権限がありません: {path}");
                MessageBox.Show($"フォルダパスへのアクセス権限がありません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string[] directories = Directory.GetDirectories(path);
            comboBox.Items.AddRange(directories);
            return true;
        }

        public static void SetDefaultSelection(ComboBox comboBox, string defaultPath)
        {
            if (!string.IsNullOrEmpty(defaultPath) && comboBox.Items.Contains(defaultPath))
            {
                comboBox.SelectedItem = defaultPath;
            }
            else if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
    }
}
