using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CreateShortCut.Utilities;

namespace CreateShortCut
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configService;

        public MainForm()
        {
            _configService = new ConfigurationService();
            InitializeComponent();

            LoggingUtility.LogInfo("=== CreateShortCut アプリケーション開始 ===");
            LoggingUtility.LogInfo("注意: ログに表示されるfile:///形式やエンコードされたパスは、.url仕様に準拠した技術的な表記です。");
            LoggingUtility.LogInfo("実際のファイル作成やパス処理は正常に動作しています。");

            InitializeComboBox();
            LinkTxt.Focus();
            // フォームの境界スタイルをFixedSingleに設定します
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // 最大化ボタンを無効にします
            this.MaximizeBox = false;
            // 最小化ボタンを無効にします
            this.MinimizeBox = false;
            // フォームを画面の中央に配置します
            this.StartPosition = FormStartPosition.CenterScreen;

            // TabIndexを設定します
            SaveFolderCmb.TabIndex = 0;
            LinkTxt.TabIndex = 1;
            NameTxt.TabIndex = 2;
            OpenFolderBtn.TabIndex = 3;
            SettingBtn.TabIndex = 4;
            CreateBtn.TabIndex = 5;

            // EnterキーでCreateBtn_Clickが実行されるように設定
            this.AcceptButton = CreateBtn;
        }

        private void InitializeComboBox()
        {
            string path = _configService.GetFolderPath();

            try
            {
                ComboBoxHelper.LoadDirectories(SaveFolderCmb, path);
            }
            catch (UnauthorizedAccessException ex)
            {
                ExceptionHandler.HandleAccessDenied(ex, "InitializeComboBox");
            }
            catch (DirectoryNotFoundException ex)
            {
                ExceptionHandler.HandleException(ex, "ディレクトリが見つかりません");
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleIOException(ex, "InitializeComboBox");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, "予期しないエラー");
            }
            finally
            {
                // デフォルトパスの設定
                string defaultPath = _configService.GetDefaultPath();
                if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
                {
                    ComboBoxHelper.SetDefaultSelection(SaveFolderCmb, defaultPath);
                }
                else if (SaveFolderCmb.Items.Count > 0)
                {
                    SaveFolderCmb.SelectedIndex = 0;
                }

                // デフォルトパスが設定されていないか存在しない場合の警告
                if (string.IsNullOrEmpty(defaultPath) || !Directory.Exists(defaultPath))
                {
                    string adminWarning = SecurityUtility.IsRunningAsAdministrator() ? "" : "\n\nなお、設定の保存には管理者権限が必要です。このアプリを管理者として再起動することをお勧めします。";
                    LoggingUtility.LogWarn($"デフォルトパスが設定されていないか、存在しません: {defaultPath}");
                    MessageBox.Show($"デフォルトパスが設定されていないか、存在しません。\n設定画面でデフォルトパスを設定してください。{adminWarning}", "設定が必要", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void CreateShortcut(string shortcutPath, string url)
        {
            try
            {
                // ラジオボタンの選択状態に基づいてプレフィックスを決定（角括弧を丸括弧に変更）
                string prefix = LocalRadioBtn.Checked ? "(Local)" : "(Remote)";

                // ローカル/リモートの候補を半角空白区切りで連結
                string suffix = LocalRadioBtn.Checked ?
                    " local ローカル ろ ろーかる ro-karu ro- ろー" :
                    " Remote リモート り りもーと rimo-to rimoto rimo りもー りも";

                // ファイル名を構築してサニタイズ
                string rawFileName = $"{prefix}{NameTxt.Text}{suffix}";
                string sanitizedFileName = SanitizeFileName(rawFileName);

                // URLファイルのパスを作成
                string urlFilePath = Path.Combine(shortcutPath, $"{sanitizedFileName}.url");

                LoggingUtility.LogInfo($"ショートカット作成: {sanitizedFileName}.url");
                LoggingUtility.LogDebug($"受信URL: {url}");
                LoggingUtility.LogDebug($"表示用パス: {GetUserFriendlyPath(url)}");

                string finalUrl;

                // URLまたはfile:///形式かによって処理を分ける
                if (url.StartsWith("file:///"))
                {
                    finalUrl = url;
                    LoggingUtility.LogDebug($"file:///形式のURL使用: {finalUrl}");
                    LoggingUtility.LogDebug($"file:///表示用パス: {GetUserFriendlyPath(finalUrl)}");
                }
                else
                {
                    finalUrl = Uri.EscapeUriString(url);
                    LoggingUtility.LogDebug($"HTTP/HTTPSエスケープ後URL: {finalUrl}");
                    LoggingUtility.LogDebug($"HTTP/HTTPS表示用URL: {GetUserFriendlyPath(finalUrl)}");
                }

                // URLファイルの内容を作成
                string urlFileContent = $"[InternetShortcut]{Environment.NewLine}URL={finalUrl}";

                LoggingUtility.LogDebug($".urlファイル作成完了: {urlFilePath}");

                // URLファイルを作成（Shift-JISエンコーディングで統一）
                System.IO.File.WriteAllText(urlFilePath, urlFileContent, Encoding.GetEncoding("Shift_JIS"));

                MessageBox.Show(".urlファイルが作成されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UriFormatException ex)
            {
                LoggingUtility.LogError($"URL形式エラー: {ex.Message}", ex);
                MessageBox.Show("URLの形式が正しくありません。\n\n" + ex.Message, "URL形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ショートカット作成エラー: {ex.Message}", ex);
                MessageBox.Show("ショートカットの作成中にエラーが発生しました。\n\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput(out string inputValue, out string selectedPath)
        {
            inputValue = null;
            selectedPath = null;

            if (SaveFolderCmb.SelectedItem == null)
            {
                MessageBox.Show("保存先フォルダを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            selectedPath = SaveFolderCmb.SelectedItem.ToString();

            if (string.IsNullOrEmpty(LinkTxt.Text) || string.IsNullOrEmpty(NameTxt.Text))
            {
                MessageBox.Show("リンクまたはパス、およびショートカット名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            inputValue = LinkTxt.Text.Trim();
            return true;
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out string inputValue, out string selectedPath))
            {
                return;
            }

            // 入力値の種類を判定
            bool? inputType = PathUtility.ClassifyInput(inputValue);
            if (inputType == null)
            {
                MessageBox.Show("入力された値が正しくありません。\n\n対応形式:\n・URL: https://example.com\n・ローカルパス: C:\\Users\\user\\Documents\\file.txt\n・相対パス: .\\folder\\file.txt", "入力形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string finalUrl;

            if (inputType == true) // URL形式
            {
                LoggingUtility.LogDebug($"URL形式として処理: {inputValue}");
                finalUrl = inputValue;
            }
            else // ローカルパス形式
            {
                LoggingUtility.LogDebug($"ローカルパス形式として処理: {inputValue}");

                if (!PathUtility.ValidateAccess(inputValue, out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "ファイル/ディレクトリアクセスエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                finalUrl = ConvertToValidUrl(inputValue);
                if (string.IsNullOrEmpty(finalUrl))
                {
                    string displayPath = GetUserFriendlyPath(inputValue);
                    MessageBox.Show($"ローカルパスの変換に失敗しました。\n\nパス: {displayPath}", "変換エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoggingUtility.LogDebug($"file:///形式に変換完了: {finalUrl}");
                LoggingUtility.LogDebug($"変換完了表示用パス: {GetUserFriendlyPath(finalUrl)}");
            }

            LoggingUtility.LogInfo($"ショートカット作成開始 - 最終URL: {finalUrl}");
            LoggingUtility.LogDebug($"ショートカット作成 - 表示用パス: {GetUserFriendlyPath(finalUrl)}");
            CreateShortcut(selectedPath, finalUrl);
        }

        private void ConfirmAndExit(string logMessage)
        {
            var result = MessageBox.Show(
                "アプリケーションを終了しますか？",
                "終了確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                LoggingUtility.LogInfo(logMessage);
                Application.Exit();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl + W でアプリケーション終了（確認ダイアログ付き）
            if (keyData == (Keys.Control | Keys.W))
            {
                try
                {
                    ConfirmAndExit("Ctrl+W によるアプリケーション終了");
                    return true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"Ctrl+W処理エラー: {ex.Message}", ex);
                    this.Close();
                    return true;
                }
            }

            // Esc でアプリケーション終了（確認ダイアログ付き）
            if (keyData == Keys.Escape)
            {
                try
                {
                    ConfirmAndExit("Escキーによるアプリケーション終了");
                    return true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"Escキー処理エラー: {ex.Message}", ex);
                    this.Close();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FolderBtn_Click(object sender, EventArgs e)
        {
            // SettingFormをインスタンス化して表示する
            using (var settingForm = new SettingForm())
            {
                settingForm.ShowDialog();
                InitializeComboBox(); // 画面情報を更新するためにInitializeComboBoxメソッドを呼び出す
                LinkTxt.Focus();
            }
        }

        private void OpenFolderBtn_Click(object sender, EventArgs e)
        {
            string selectedPath = null;

            try
            {
                if (SaveFolderCmb.SelectedItem == null)
                {
                    MessageBox.Show("保存先フォルダが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedPath = SaveFolderCmb.SelectedItem.ToString();

                if (!Directory.Exists(selectedPath))
                {
                    LoggingUtility.LogError($"選択されたフォルダが存在しません: {selectedPath}");
                    MessageBox.Show($"選択されたフォルダが存在しません:\n{selectedPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Process.Start("explorer.exe", selectedPath);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"フォルダを開く際にエラーが発生しました: {ex.Message}", ex);
                MessageBox.Show($"フォルダを開く際にエラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ローカルパスをfile:///形式のURIに変換する
        /// </summary>
        private string ConvertToValidUrl(string localPath)
        {
            try
            {
                string absolutePath = PathUtility.ResolveAbsolutePath(localPath);

                // 手動でfile:///形式のURLを構築（UTF-8パーセントエンコーディングを回避）
                string urlPath = absolutePath.Replace('\\', '/');
                urlPath = urlPath.Replace(" ", "%20");
                string fileUrl = "file:///" + urlPath;

                LoggingUtility.LogDebug($"ローカルパス変換: {localPath} → {fileUrl}");
                LoggingUtility.LogDebug($"変換後表示用パス: {GetUserFriendlyPath(fileUrl)}");
                return fileUrl;
            }
            catch (ArgumentException ex)
            {
                LoggingUtility.LogError($"パス引数エラー: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (NotSupportedException ex)
            {
                LoggingUtility.LogError($"パス形式サポート外: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (PathTooLongException ex)
            {
                LoggingUtility.LogError($"パスが長すぎます: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ConvertToValidUrl変換エラー: {localPath} - {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// ログ表示用にエンコードされたURLを読みやすいパスに変換する
        /// </summary>
        private string GetUserFriendlyPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            try
            {
                if (url.StartsWith("file:///"))
                {
                    Uri uri = new Uri(url);
                    return Uri.UnescapeDataString(uri.LocalPath);
                }

                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    return Uri.UnescapeDataString(url);
                }

                return url;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"URLデコードエラー: {url} - {ex.Message}");
                return url;
            }
        }

        /// <summary>
        /// ファイル名をWindowsファイルシステム用にサニタイズする
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "無題";
            }

            try
            {
                char[] invalidChars = Path.GetInvalidFileNameChars();
                string sanitized = fileName;

                foreach (char invalidChar in invalidChars)
                {
                    sanitized = sanitized.Replace(invalidChar, '_');
                }

                sanitized = sanitized.Replace('[', '(')
                                   .Replace(']', ')')
                                   .Replace('<', '(')
                                   .Replace('>', ')')
                                   .Replace('|', '_')
                                   .Replace('?', '_')
                                   .Replace('*', '_');

                sanitized = sanitized.Trim().Trim('.');

                if (string.IsNullOrWhiteSpace(sanitized))
                {
                    sanitized = "無題";
                }

                if (sanitized.Length > 240)
                {
                    sanitized = sanitized.Substring(0, 240);
                    LoggingUtility.LogDebug($"ファイル名が長すぎるため切り詰めました: {sanitized.Length}文字に短縮");
                }

                string[] reservedNames = { "CON", "PRN", "AUX", "NUL",
                    "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                    "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

                string upperName = sanitized.ToUpper();
                if (reservedNames.Contains(upperName))
                {
                    sanitized = "_" + sanitized;
                    LoggingUtility.LogDebug($"予約語を回避: _{sanitized}");
                }

                LoggingUtility.LogDebug($"ファイル名サニタイズ完了: '{fileName}' → '{sanitized}'");
                return sanitized;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ファイル名サニタイズエラー: {ex.Message}", ex);
                return "無題_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }
        }

    }
}
