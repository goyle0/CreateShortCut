using System;
using System.IO;
using System.Windows.Forms;
using CreateShortCut.Utilities;

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
            string folderPath = _configService.GetFolderPath();

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
            if (string.IsNullOrEmpty(FolderLbl.Text) || string.IsNullOrEmpty(DefaultFolderCmb.Text))
            {
                MessageBox.Show("フォルダパスとデフォルトのフォルダ設定を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 管理者権限チェック
            if (!SecurityUtility.IsRunningAsAdministrator())
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
                _configService.SetDefaultPath(DefaultFolderCmb.Text);
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

            if (FolderFlg)
            {
                path = FolderLbl.Text;
            }
            else
            {
                path = _configService.GetFolderPath();
            }

            try
            {
                ComboBoxHelper.LoadDirectories(DefaultFolderCmb, path);
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
                if (FolderFlg)
                {
                    if (DefaultFolderCmb.Items.Count > 0)
                    {
                        DefaultFolderCmb.SelectedIndex = 0;
                    }
                }
                else
                {
                    string defaultPath = _configService.GetDefaultPath();
                    ComboBoxHelper.SetDefaultSelection(DefaultFolderCmb, defaultPath);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl + W でアプリケーション終了（設定画面からの確認ダイアログ付き）
            if (keyData == (Keys.Control | Keys.W))
            {
                try
                {
                    var result = MessageBox.Show(
                        "設定を保存せずにアプリケーションを終了しますか？",
                        "終了確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        LoggingUtility.LogInfo("Ctrl+W による設定画面からのアプリケーション終了");
                        Application.Exit();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"設定画面Ctrl+W処理エラー: {ex.Message}", ex);
                    this.Close();
                    return true;
                }
            }

            // Esc で設定画面を閉じる
            if (keyData == Keys.Escape)
            {
                try
                {
                    this.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"設定画面Escキー処理エラー: {ex.Message}", ex);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
