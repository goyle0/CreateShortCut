using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CreateShortCut
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
            string path = ConfigurationManager.AppSettings["FolderPath"];

            SaveFolderCmb.Items.Clear();

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
                SaveFolderCmb.Items.AddRange(directories);
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
                // デフォルトパスの設定
                string defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
                if (SaveFolderCmb.Items.Contains(defaultPath))
                {
                    SaveFolderCmb.SelectedItem = defaultPath;
                }
                else if (SaveFolderCmb.Items.Count > 0)
                {
                    SaveFolderCmb.SelectedIndex = 0;
                }
            }
        }

        private void CreateShortcut(string shortcutPath)
        {
            try
            {
                // URLファイルのパスを作成
                string urlFilePath = Path.Combine(shortcutPath, $"{NameTxt.Text}.url");

                // URLファイルの内容を作成
                string urlFileContent = $"[InternetShortcut]{Environment.NewLine}URL={LinkTxt.Text}";

                // URLファイルを作成（日本語文字化け防止のためShift-JISエンコーディングを使用）
                System.IO.File.WriteAllText(urlFilePath, urlFileContent, Encoding.GetEncoding("Shift_JIS"));

                MessageBox.Show(".urlファイルが作成されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogError($"ショートカット作成エラー: {ex.Message}", ex);
                MessageBox.Show("ショートカットの作成中にエラーが発生しました。\n\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            string selectedPath = SaveFolderCmb.SelectedItem.ToString();

            if (string.IsNullOrEmpty(LinkTxt.Text) || string.IsNullOrEmpty(NameTxt.Text))
            {
                MessageBox.Show("リンクとショートカット名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 指定された場所にショートカットファイルを生成する
            CreateShortcut(selectedPath);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
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
            try
            {
                // 選択されているフォルダパスを取得
                if (SaveFolderCmb.SelectedItem == null)
                {
                    MessageBox.Show("保存先フォルダが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedPath = SaveFolderCmb.SelectedItem.ToString();

                // フォルダの存在確認
                if (!Directory.Exists(selectedPath))
                {
                    LogError($"選択されたフォルダが存在しません: {selectedPath}");
                    MessageBox.Show($"選択されたフォルダが存在しません:\n{selectedPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // エクスプローラーでフォルダを開く
                Process.Start("explorer.exe", selectedPath);
            }
            catch (Exception ex)
            {
                LogError($"フォルダを開く際にエラーが発生しました: {ex.Message}", ex);
                MessageBox.Show($"フォルダを開く際にエラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
                File.AppendAllText(logPath, logMessage, Encoding.GetEncoding("Shift_JIS"));
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
