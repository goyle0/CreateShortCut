using System;
using System.Configuration;
using System.IO;
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
                config.AppSettings.Settings["FolderPath"].Value = FolderLbl.Text;
                config.AppSettings.Settings["DefaultPath"].Value = DefalutFolderCmb.Text;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

               MessageBox.Show("設定が更新されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // エラーメッセージを表示
                MessageBox.Show(ex.Message);
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
                // 指定したパスの下にあるすべてのディレクトリを取得します
                string[] directories = Directory.GetDirectories(path);

                // ComboBoxにディレクトリ名を追加します
                DefalutFolderCmb.Items.AddRange(directories);
            }
            catch (IOException ex)
            {
                // ディレクトリの取得に失敗した場合、例外をキャッチします
                MessageBox.Show(ex.Message);
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
                    // ComboBoxのデフォルト値を設定します
                    DefalutFolderCmb.SelectedItem = ConfigurationManager.AppSettings["DefaultPath"];
                }
                
            }
        }
    }
}
