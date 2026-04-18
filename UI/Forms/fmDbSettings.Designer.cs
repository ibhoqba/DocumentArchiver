namespace DocumentArchiever.Setting
{
    partial class fmDbSettings
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
            label2 = new Label();
            label1 = new Label();
            txtPwd = new TextBox();
            txtUserName = new TextBox();
            btnSave = new Button();
            btnExit = new Button();
            label3 = new Label();
            label4 = new Label();
            txtDbName = new TextBox();
            txtServer = new TextBox();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(34, 119);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 11;
            label2.Text = "كلمة المرور";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 93);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 10;
            label1.Text = "المستخدم";
            // 
            // txtPwd
            // 
            txtPwd.Location = new Point(143, 119);
            txtPwd.Name = "txtPwd";
            txtPwd.Size = new Size(134, 23);
            txtPwd.TabIndex = 3;
            txtPwd.TextAlign = HorizontalAlignment.Center;
            txtPwd.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(143, 90);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(134, 23);
            txtUserName.TabIndex = 2;
            txtUserName.TextAlign = HorizontalAlignment.Center;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(77, 177);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 4;
            btnSave.Text = "حفظ";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(158, 177);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 5;
            btnExit.Text = "خروج";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(34, 48);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 15;
            label3.Text = "قاعدة البيانات";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(34, 22);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 14;
            label4.Text = "السرفر /الخادم";
            // 
            // txtDbName
            // 
            txtDbName.Location = new Point(143, 48);
            txtDbName.Name = "txtDbName";
            txtDbName.Size = new Size(134, 23);
            txtDbName.TabIndex = 1;
            txtDbName.TextAlign = HorizontalAlignment.Center;
            // 
            // txtServer
            // 
            txtServer.Location = new Point(143, 19);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(134, 23);
            txtServer.TabIndex = 0;
            txtServer.TextAlign = HorizontalAlignment.Center;
            // 
            // fmDbSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(310, 213);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(txtDbName);
            Controls.Add(txtServer);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPwd);
            Controls.Add(txtUserName);
            Controls.Add(btnSave);
            Controls.Add(btnExit);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "fmDbSettings";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "اعدادات الاتصال بالقاعدة";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private TextBox txtPwd;
        private TextBox txtUserName;
        private Button btnSave;
        private Button btnExit;
        private Label label3;
        private Label label4;
        private TextBox txtDbName;
        private TextBox txtServer;
    }
}