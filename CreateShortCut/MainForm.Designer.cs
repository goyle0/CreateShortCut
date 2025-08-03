namespace CreateShortCut
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.CreateBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LinkTxt = new System.Windows.Forms.TextBox();
            this.NameTxt = new System.Windows.Forms.TextBox();
            this.SaveFolderCmb = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SettingBtn = new System.Windows.Forms.Button();
            this.OpenFolderBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CreateBtn
            // 
            this.CreateBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.CreateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.CreateBtn.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.CreateBtn.Location = new System.Drawing.Point(529, 238);
            this.CreateBtn.Name = "CreateBtn";
            this.CreateBtn.Size = new System.Drawing.Size(188, 72);
            this.CreateBtn.TabIndex = 5;
            this.CreateBtn.Text = "作　成";
            this.CreateBtn.UseVisualStyleBackColor = false;
            this.CreateBtn.Click += new System.EventHandler(this.CreateBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.label2.Location = new System.Drawing.Point(31, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "ショートカット名";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.label1.Location = new System.Drawing.Point(31, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "リンク先・パス";
            // 
            // LinkTxt
            // 
            this.LinkTxt.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.LinkTxt.Location = new System.Drawing.Point(162, 117);
            this.LinkTxt.Name = "LinkTxt";
            this.LinkTxt.Size = new System.Drawing.Size(556, 27);
            this.LinkTxt.TabIndex = 2;
            // 
            // NameTxt
            // 
            this.NameTxt.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.NameTxt.Location = new System.Drawing.Point(162, 186);
            this.NameTxt.Name = "NameTxt";
            this.NameTxt.Size = new System.Drawing.Size(556, 27);
            this.NameTxt.TabIndex = 3;
            // 
            // SaveFolderCmb
            // 
            this.SaveFolderCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SaveFolderCmb.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.SaveFolderCmb.FormattingEnabled = true;
            this.SaveFolderCmb.Location = new System.Drawing.Point(162, 53);
            this.SaveFolderCmb.Name = "SaveFolderCmb";
            this.SaveFolderCmb.Size = new System.Drawing.Size(555, 28);
            this.SaveFolderCmb.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.label3.Location = new System.Drawing.Point(32, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "保存先";
            // 
            // SettingBtn
            // 
            this.SettingBtn.BackColor = System.Drawing.Color.LightGreen;
            this.SettingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SettingBtn.Font = new System.Drawing.Font("MS UI Gothic", 11F);
            this.SettingBtn.Location = new System.Drawing.Point(316, 238);
            this.SettingBtn.Name = "SettingBtn";
            this.SettingBtn.Size = new System.Drawing.Size(188, 72);
            this.SettingBtn.TabIndex = 4;
            this.SettingBtn.Text = "設　定";
            this.SettingBtn.UseVisualStyleBackColor = false;
            this.SettingBtn.Click += new System.EventHandler(this.FolderBtn_Click);
            // 
            // OpenFolderBtn
            // 
            this.OpenFolderBtn.BackColor = System.Drawing.Color.Orange;
            this.OpenFolderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.OpenFolderBtn.Font = new System.Drawing.Font("MS UI Gothic", 11F);
            this.OpenFolderBtn.Location = new System.Drawing.Point(103, 238);
            this.OpenFolderBtn.Name = "OpenFolderBtn";
            this.OpenFolderBtn.Size = new System.Drawing.Size(188, 72);
            this.OpenFolderBtn.TabIndex = 3;
            this.OpenFolderBtn.Text = "フォルダを開く";
            this.OpenFolderBtn.UseVisualStyleBackColor = false;
            this.OpenFolderBtn.Click += new System.EventHandler(this.OpenFolderBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 325);
            this.Controls.Add(this.OpenFolderBtn);
            this.Controls.Add(this.SettingBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SaveFolderCmb);
            this.Controls.Add(this.NameTxt);
            this.Controls.Add(this.LinkTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CreateBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "ショートカット作成アプリ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CreateBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LinkTxt;
        private System.Windows.Forms.TextBox NameTxt;
        private System.Windows.Forms.ComboBox SaveFolderCmb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SettingBtn;
        private System.Windows.Forms.Button OpenFolderBtn;
    }
}

