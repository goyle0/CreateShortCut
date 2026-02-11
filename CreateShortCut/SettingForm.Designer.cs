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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FolderLbl = new System.Windows.Forms.Label();
            this.DefaultFolderLabel = new System.Windows.Forms.Label();
            this.DefaultFolderCmb = new System.Windows.Forms.ComboBox();
            this.FolderReferenceBtn = new CreateShortCut.Controls.ModernButton();
            this.UpdateBtn = new CreateShortCut.Controls.ModernButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.FolderLbl);
            this.groupBox1.Controls.Add(this.DefaultFolderLabel);
            this.groupBox1.Controls.Add(this.DefaultFolderCmb);
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
            this.FolderLbl.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.FolderLbl.ForeColor = System.Drawing.ColorTranslator.FromHtml("#202020");
            this.FolderLbl.Location = new System.Drawing.Point(224, 60);
            this.FolderLbl.Name = "FolderLbl";
            this.FolderLbl.Size = new System.Drawing.Size(0, 21);
            this.FolderLbl.TabIndex = 8;
            //
            // DefaultFolderLabel
            //
            this.DefaultFolderLabel.AutoSize = true;
            this.DefaultFolderLabel.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.DefaultFolderLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#202020");
            this.DefaultFolderLabel.Location = new System.Drawing.Point(27, 157);
            this.DefaultFolderLabel.Name = "DefaultFolderLabel";
            this.DefaultFolderLabel.Size = new System.Drawing.Size(195, 21);
            this.DefaultFolderLabel.TabIndex = 7;
            this.DefaultFolderLabel.Text = "デフォルトのフォルダ設定";
            //
            // DefaultFolderCmb
            //
            this.DefaultFolderCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DefaultFolderCmb.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.DefaultFolderCmb.FormattingEnabled = true;
            this.DefaultFolderCmb.Location = new System.Drawing.Point(228, 154);
            this.DefaultFolderCmb.Name = "DefaultFolderCmb";
            this.DefaultFolderCmb.Size = new System.Drawing.Size(243, 29);
            this.DefaultFolderCmb.TabIndex = 5;
            //
            // FolderReferenceBtn
            //
            this.FolderReferenceBtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#6B69D6");
            this.FolderReferenceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FolderReferenceBtn.FlatAppearance.BorderSize = 0;
            this.FolderReferenceBtn.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.FolderReferenceBtn.ForeColor = System.Drawing.Color.White;
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
            this.UpdateBtn.BackColor = System.Drawing.ColorTranslator.FromHtml("#0078D4");
            this.UpdateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateBtn.FlatAppearance.BorderSize = 0;
            this.UpdateBtn.Font = new System.Drawing.Font("Yu Gothic UI", 12F);
            this.UpdateBtn.ForeColor = System.Drawing.Color.White;
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
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F9F9F9");
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

        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.ModernButton FolderReferenceBtn;
        private System.Windows.Forms.ComboBox DefaultFolderCmb;
        private System.Windows.Forms.Label DefaultFolderLabel;
        private Controls.ModernButton UpdateBtn;
        private System.Windows.Forms.Label FolderLbl;
    }
}
