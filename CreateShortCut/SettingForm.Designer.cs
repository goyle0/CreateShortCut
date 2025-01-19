namespace CreateShortCut
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FolderLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DefalutFolderCmb = new System.Windows.Forms.ComboBox();
            this.FolderReferenceBtn = new System.Windows.Forms.Button();
            this.UpdateBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FolderLbl);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.DefalutFolderCmb);
            this.groupBox1.Controls.Add(this.FolderReferenceBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 202);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "フォルダ設定";
            // 
            // FolderLbl
            // 
            this.FolderLbl.AutoSize = true;
            this.FolderLbl.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.FolderLbl.Location = new System.Drawing.Point(224, 60);
            this.FolderLbl.Name = "FolderLbl";
            this.FolderLbl.Size = new System.Drawing.Size(0, 20);
            this.FolderLbl.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.label3.Location = new System.Drawing.Point(27, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "デフォルトのフォルダ設定";
            // 
            // DefalutFolderCmb
            // 
            this.DefalutFolderCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DefalutFolderCmb.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.DefalutFolderCmb.FormattingEnabled = true;
            this.DefalutFolderCmb.Location = new System.Drawing.Point(228, 154);
            this.DefalutFolderCmb.Name = "DefalutFolderCmb";
            this.DefalutFolderCmb.Size = new System.Drawing.Size(243, 28);
            this.DefalutFolderCmb.TabIndex = 5;
            // 
            // FolderReferenceBtn
            // 
            this.FolderReferenceBtn.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.FolderReferenceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.FolderReferenceBtn.Font = new System.Drawing.Font("MS UI Gothic", 11F);
            this.FolderReferenceBtn.Location = new System.Drawing.Point(31, 35);
            this.FolderReferenceBtn.Name = "FolderReferenceBtn";
            this.FolderReferenceBtn.Size = new System.Drawing.Size(152, 73);
            this.FolderReferenceBtn.TabIndex = 1;
            this.FolderReferenceBtn.Text = "フォルダ参照設定";
            this.FolderReferenceBtn.UseVisualStyleBackColor = false;
            this.FolderReferenceBtn.Click += new System.EventHandler(this.FolderReferenceBtn_Click);
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.UpdateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.UpdateBtn.Font = new System.Drawing.Font("MS UI Gothic", 15F);
            this.UpdateBtn.Location = new System.Drawing.Point(417, 299);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(188, 72);
            this.UpdateBtn.TabIndex = 4;
            this.UpdateBtn.Text = "適　用";
            this.UpdateBtn.UseVisualStyleBackColor = false;
            this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 383);
            this.Controls.Add(this.UpdateBtn);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingForm";
            this.Text = "設定";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button FolderReferenceBtn;
        private System.Windows.Forms.ComboBox DefalutFolderCmb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.Label FolderLbl;
    }
}