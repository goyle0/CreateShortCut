﻿using System;
using System.IO;
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
        }

        private void InitializeComboBox()
        {
            string path = @"C:\Users\fsyou";

            try
            {
                // 指定したパスの下にあるすべてのディレクトリを取得します
                string[] directories = Directory.GetDirectories(path);

                // ComboBoxにディレクトリ名を追加します
                SaveFolderCmb.Items.AddRange(directories);
            }
            catch (IOException ex)
            {
                // ディレクトリの取得に失敗した場合、例外をキャッチします
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // ComboBoxのデフォルト値を設定します
                SaveFolderCmb.SelectedItem = @"C:\Users\fsyou\ShortCut";
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

                // URLファイルを作成
                System.IO.File.WriteAllText(urlFilePath, urlFileContent);

                MessageBox.Show(".urlファイルが作成されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
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
    }
}
