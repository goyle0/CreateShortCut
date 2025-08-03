using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace CreateShortCut
{
    public partial class SettingForm : Form
    {
        private readonly IConfigurationService _configService;
        
        public SettingForm()
        {
            _configService = new ConfigurationService();
            InitializeComponent();
            FolderLbl.Text = _configService.GetFolderPath();

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

            // 管理者権限チェック
            if (!IsRunningAsAdministrator())
            {
                LoggingUtility.LogError("設定保存には管理者権限が必要です");
                MessageBox.Show("設定の保存には管理者権限が必要です。\nこのアプリを管理者として再度起動してください。", "管理者権限が必要", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 設定ファイルの書き込み権限チェック
                var configService = _configService as ConfigurationService;
                string configPath = configService.GetConfigFilePath();
                if (!_configService.HasFileWriteAccess(configPath))
                {
                    LoggingUtility.LogError($"設定ファイルへの書き込み権限がありません: {configPath}");
                    MessageBox.Show($"設定ファイルへの書き込み権限がありません。\nこのアプリを管理者として再度起動してください。\n\nファイル: {configPath}", "アクセス拒否", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                _configService.SetFolderPath(FolderLbl.Text);
                _configService.SetDefaultPath(DefalutFolderCmb.Text);
                _configService.SaveConfiguration();

               MessageBox.Show("設定が更新されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                LoggingUtility.LogError($"設定ファイルアクセス拒否: {ex.Message}", ex);
                MessageBox.Show($"設定ファイルへのアクセスが拒否されました。\nこのアプリを管理者として再度起動してください。\n\nエラー: {ex.Message}", "アクセス拒否", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"設定更新エラー: {ex.Message}", ex);
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
                path = _configService.GetFolderPath();
            }

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
                DefalutFolderCmb.Items.AddRange(directories);
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
                    string defaultPath = _configService.GetDefaultPath();
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl + W でアプリケーション終了（設定画面からの確認ダイアログ付き）
            if (keyData == (Keys.Control | Keys.W))
            {
                try
                {
                    // 設定画面からのアプリ終了確認（未保存変更に関する警告）
                    var result = MessageBox.Show(
                        "設定を保存せずにアプリケーションを終了しますか？",
                        "終了確認", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1); // はいがデフォルト
                        
                    if (result == DialogResult.Yes)
                    {
                        LoggingUtility.LogError("Ctrl+W による設定画面からのアプリケーション終了");
                        Application.Exit();
                    }
                    
                    return true; // キー処理完了を通知
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"設定画面Ctrl+W処理エラー: {ex.Message}", ex);
                    // エラー時はフォールバック（設定画面のみ閉じる）
                    this.Close();
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
