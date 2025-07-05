using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CreateShortCut
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            FolderLbl.Text = ConfigurationManager.AppSettings["FolderPath"];

            InitializeComboBox();
        }
        private void FolderReferenceBtn_Click(object sender, EventArgs e)
        {
            // フォルダパスを取得
            string folderPath = ConfigurationManager.AppSettings["FolderPath"];

            // フォルダパスを設定するダイアログを表示
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = folderPath;
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // 選択されたフォルダパスを取得
                    folderPath = dialog.SelectedPath;
                    FolderLbl.Text = folderPath;

                    InitializeComboBox(true);
                }
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FolderLbl.Text)|| string.IsNullOrEmpty(DefalutFolderCmb.Text))
            {
                MessageBox.Show("フォルダパスとデフォルトのフォルダ設定を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                // App.configのパス設定を更新
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                // 設定ファイルの書き込み権限チェック
                string configPath = config.FilePath;
                if (!HasFileWriteAccess(configPath))
                {
                    LogError($"設定ファイルへの書き込み権限がありません: {configPath}");
                    MessageBox.Show($"設定ファイルへの書き込み権限がありません。\nこのアプリを管理者として再度起動してください。\n\nファイル: {configPath}", "アクセス拒否", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                config.AppSettings.Settings["FolderPath"].Value = FolderLbl.Text;
                config.AppSettings.Settings["DefaultPath"].Value = DefalutFolderCmb.Text;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

               MessageBox.Show("設定が更新されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError($"設定ファイルアクセス拒否: {ex.Message}", ex);
                MessageBox.Show($"設定ファイルへのアクセスが拒否されました。\nこのアプリを管理者として再度起動してください。\n\nエラー: {ex.Message}", "アクセス拒否", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LogError($"設定更新エラー: {ex.Message}", ex);
                MessageBox.Show($"設定の更新中にエラーが発生しました。\n\nエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBox(bool FolderFlg = false)
        {
            string path;
            
            DefalutFolderCmb.Items.Clear();
            
            if (FolderFlg)
            {
                // app.configのFolderパス情報を取得
                path = FolderLbl.Text;
            }
            else
            {
                // app.configのFolderパス情報を取得
                path = ConfigurationManager.AppSettings["FolderPath"];
            }

            try
            {
                // パスの存在確認
                if (!Directory.Exists(path))
                {
                    LogError($"指定されたフォルダパスが存在しません: {path}");
                    MessageBox.Show($"指定されたフォルダパスが存在しません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // アクセス権限の確認
                if (!HasDirectoryAccess(path))
                {
                    LogError($"フォルダパスへのアクセス権限がありません: {path}");
                    MessageBox.Show($"フォルダパスへのアクセス権限がありません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 指定したパスの下にあるすべてのディレクトリを取得します
                string[] directories = Directory.GetDirectories(path);

                // ComboBoxにディレクトリ名を追加します
                DefalutFolderCmb.Items.AddRange(directories);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError($"アクセス拒否エラー: {ex.Message}", ex);
                MessageBox.Show($"アクセス拒否: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                LogError($"ディレクトリが見つかりません: {ex.Message}", ex);
                MessageBox.Show($"ディレクトリが見つかりません: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                LogError($"IO エラー: {ex.Message}", ex);
                MessageBox.Show($"IO エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LogError($"予期しないエラー: {ex.Message}", ex);
                MessageBox.Show($"予期しないエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (FolderFlg)
                {
                    if (DefalutFolderCmb.Items.Count > 0)
                    {
                        // ComboBoxのデフォルト値を設定します
                        DefalutFolderCmb.SelectedIndex = 0;
                    }
                }
                else
                {
                    // デフォルトパスの設定
                    string defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
                    if (DefalutFolderCmb.Items.Contains(defaultPath))
                    {
                        DefalutFolderCmb.SelectedItem = defaultPath;
                    }
                    else if (DefalutFolderCmb.Items.Count > 0)
                    {
                        DefalutFolderCmb.SelectedIndex = 0;
                    }
                }
                
            }
        }
        private bool HasDirectoryAccess(string path)
        {
            try
            {
                Directory.GetDirectories(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool HasFileWriteAccess(string filePath)
        {
            try
            {
                // ファイルの存在確認
                if (!File.Exists(filePath))
                {
                    return false;
                }
                
                // 書き込み権限のテスト
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write))
                {
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        private void LogError(string message)
        {
            LogError(message, null);
        }
        
        private void LogError(string message, Exception ex)
        {
            try
            {
                string logMessage = FormatLogMessage(message, ex);
                
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
        
        private bool HasDirectoryWriteAccess(string directoryPath)
        {
            try
            {
                string testFile = Path.Combine(directoryPath, "test_write_access.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private bool TryWriteLog(string logPath, string logMessage)
        {
            try
            {
                // ディレクトリが存在しない場合は作成する
                string directory = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.AppendAllText(logPath, logMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private string FormatLogMessage(string message, Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
            
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
            }
            
            sb.AppendLine($"Application Path: {Application.StartupPath}");
            sb.AppendLine($"Working Directory: {Directory.GetCurrentDirectory()}");
            sb.AppendLine($"User: {Environment.UserName}");
            sb.AppendLine($"Machine: {Environment.MachineName}");
            sb.AppendLine($"OS Version: {Environment.OSVersion}");
            sb.AppendLine($"CLR Version: {Environment.Version}");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine();
            
            return sb.ToString();
        }
    }
}
