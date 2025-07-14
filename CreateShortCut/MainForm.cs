using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace CreateShortCut
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configService;
        
        public MainForm()
        {
            _configService = new ConfigurationService();
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
            string path = _configService.GetFolderPath();

            SaveFolderCmb.Items.Clear();

            try
            {
                // パスの存在確認
                if (!Directory.Exists(path))
                {
                    LoggingUtility.LogError($"指定されたフォルダパスが存在しません: {path}");
                    MessageBox.Show($"指定されたフォルダパスが存在しません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // アクセス権限の確認
                if (!HasDirectoryAccess(path))
                {
                    LoggingUtility.LogError($"フォルダパスへのアクセス権限がありません: {path}");
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
                LoggingUtility.LogError($"アクセス拒否エラー: {ex.Message}", ex);
                MessageBox.Show($"アクセス拒否: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                LoggingUtility.LogError($"ディレクトリが見つかりません: {ex.Message}", ex);
                MessageBox.Show($"ディレクトリが見つかりません: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                LoggingUtility.LogError($"IO エラー: {ex.Message}", ex);
                MessageBox.Show($"IO エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"予期しないエラー: {ex.Message}", ex);
                MessageBox.Show($"予期しないエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // デフォルトパスの設定
                string defaultPath = _configService.GetDefaultPath();
                if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath) && SaveFolderCmb.Items.Contains(defaultPath))
                {
                    SaveFolderCmb.SelectedItem = defaultPath;
                }
                else if (SaveFolderCmb.Items.Count > 0)
                {
                    SaveFolderCmb.SelectedIndex = 0;
                }
                
                // デフォルトパスが設定されていないか存在しない場合の警告
                if (string.IsNullOrEmpty(defaultPath) || !Directory.Exists(defaultPath))
                {
                    string adminWarning = IsRunningAsAdministrator() ? "" : "\n\nなお、設定の保存には管理者権限が必要です。このアプリを管理者として再起動することをお勧めします。";
                    LoggingUtility.LogError($"デフォルトパスが設定されていないか、存在しません: {defaultPath}");
                    MessageBox.Show($"デフォルトパスが設定されていないか、存在しません。\n設定画面でデフォルトパスを設定してください。{adminWarning}", "設定が必要", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                LoggingUtility.LogError($"ショートカット作成エラー: {ex.Message}", ex);
                MessageBox.Show("ショートカットの作成中にエラーが発生しました。\n\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (SaveFolderCmb.SelectedItem == null)
            {
                MessageBox.Show("保存先フォルダを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string selectedPath = SaveFolderCmb.SelectedItem.ToString();

            if (string.IsNullOrEmpty(LinkTxt.Text) || string.IsNullOrEmpty(NameTxt.Text))
            {
                MessageBox.Show("リンクとショートカット名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // URL形式の検証
            if (!Uri.TryCreate(LinkTxt.Text, UriKind.Absolute, out Uri uriResult) || 
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                MessageBox.Show("正しいURL形式を入力してください。\n（例：https://example.com）", "URL形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    LoggingUtility.LogError($"選択されたフォルダが存在しません: {selectedPath}");
                    MessageBox.Show($"選択されたフォルダが存在しません:\n{selectedPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // エクスプローラーでフォルダを開く
                Process.Start("explorer.exe", selectedPath);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"フォルダを開く際にエラーが発生しました: {ex.Message}", ex);
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


        private bool IsRunningAsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}
