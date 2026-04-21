namespace DocumentArchiever.UI.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblStatus = new Label();
            cboScanners = new ComboBox();
            btnRefreshScanners = new Button();
            btnScan = new Button();
            btnCancel = new Button();
            progressBar1 = new ProgressBar();
            lblInfo = new Label();
            lblErrors = new Label();
            groupBox1 = new GroupBox();
            rbBlackWhite = new RadioButton();
            rbGrayscale = new RadioButton();
            rbColor = new RadioButton();
            chkDuplex = new CheckBox();
            txtDPI = new TextBox();
            lblDPI = new Label();
            chkContinuousScan = new CheckBox();
            btnRefresh = new Button();
            pnlSearch = new Panel();
            btnSaveToDatabase = new Button();
            lstUnidentified = new ListBox();
            webBrowserPdf = new WebBrowser();
            btnSearch = new Button();
            txtUniqueNumber = new TextBox();
            lblExpress = new Label();
            DocGroupBox = new GroupBox();
            chkWithoutIdentity = new CheckBox();
            dtTime = new DateTimePicker();
            txtMissedNumbers = new TextBox();
            txtBtwBranch = new TextBox();
            cbBtwBranches = new CheckBox();
            lblDocStartNumber = new Label();
            txtStartDocNumber = new TextBox();
            cbBranches = new ComboBox();
            cbCashDocs = new CheckBox();
            BtnGetFile = new Button();
            btnGetAllExpress = new Button();
            BtnRandomSave = new Button();
            BtnGet1Express = new Button();
            menuStrip1 = new MenuStrip();
            BtnEditDeleteLastScanned = new ToolStripMenuItem();
            BtnSearchDoc = new ToolStripMenuItem();
            BtnUploadFile = new ToolStripMenuItem();
            tsmiSettings = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            statuslbl1 = new ToolStripStatusLabel();
            statuslbl2 = new ToolStripStatusLabel();
            btnTestQueue = new Button();
            groupBox1.SuspendLayout();
            pnlSearch.SuspendLayout();
            DocGroupBox.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(14, 35);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(37, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "«·Õ«·…:";
            // 
            // cboScanners
            // 
            cboScanners.DropDownStyle = ComboBoxStyle.DropDownList;
            cboScanners.FormattingEnabled = true;
            cboScanners.Location = new Point(14, 53);
            cboScanners.Margin = new Padding(4, 3, 4, 3);
            cboScanners.Name = "cboScanners";
            cboScanners.Size = new Size(349, 23);
            cboScanners.TabIndex = 1;
            // 
            // btnRefreshScanners
            // 
            btnRefreshScanners.Location = new Point(371, 51);
            btnRefreshScanners.Margin = new Padding(4, 3, 4, 3);
            btnRefreshScanners.Name = "btnRefreshScanners";
            btnRefreshScanners.Size = new Size(117, 27);
            btnRefreshScanners.TabIndex = 2;
            btnRefreshScanners.Text = " ÕœÌÀ «·„«”Õ« ";
            btnRefreshScanners.UseVisualStyleBackColor = true;
            btnRefreshScanners.Click += btnRefreshScanners_Click;
            // 
            // btnScan
            // 
            btnScan.Location = new Point(268, 138);
            btnScan.Margin = new Padding(4, 3, 4, 3);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(117, 35);
            btnScan.TabIndex = 3;
            btnScan.Text = "»œ¡ «·„”Õ";
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += btnScan_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(14, 138);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(117, 35);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "≈·€«¡";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(14, 231);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(467, 27);
            progressBar1.TabIndex = 5;
            progressBar1.Visible = false;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(14, 277);
            lblInfo.Margin = new Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(39, 15);
            lblInfo.TabIndex = 6;
            lblInfo.Text = "Ã«Â“...";
            // 
            // lblErrors
            // 
            lblErrors.ForeColor = Color.Red;
            lblErrors.Location = new Point(14, 300);
            lblErrors.Margin = new Padding(4, 0, 4, 0);
            lblErrors.Name = "lblErrors";
            lblErrors.Size = new Size(467, 69);
            lblErrors.TabIndex = 7;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbBlackWhite);
            groupBox1.Controls.Add(rbGrayscale);
            groupBox1.Controls.Add(rbColor);
            groupBox1.Location = new Point(14, 81);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(175, 51);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "·Ê‰ «·’Ê—…";
            // 
            // rbBlackWhite
            // 
            rbBlackWhite.AutoSize = true;
            rbBlackWhite.Location = new Point(105, 22);
            rbBlackWhite.Margin = new Padding(4, 3, 4, 3);
            rbBlackWhite.Name = "rbBlackWhite";
            rbBlackWhite.Size = new Size(50, 19);
            rbBlackWhite.TabIndex = 2;
            rbBlackWhite.Text = "√”Êœ";
            // 
            // rbGrayscale
            // 
            rbGrayscale.AutoSize = true;
            rbGrayscale.Location = new Point(52, 22);
            rbGrayscale.Margin = new Padding(4, 3, 4, 3);
            rbGrayscale.Name = "rbGrayscale";
            rbGrayscale.Size = new Size(54, 19);
            rbGrayscale.TabIndex = 1;
            rbGrayscale.Text = "—„«œÌ";
            // 
            // rbColor
            // 
            rbColor.AutoSize = true;
            rbColor.Checked = true;
            rbColor.Location = new Point(7, 22);
            rbColor.Margin = new Padding(4, 3, 4, 3);
            rbColor.Name = "rbColor";
            rbColor.Size = new Size(50, 19);
            rbColor.TabIndex = 0;
            rbColor.TabStop = true;
            rbColor.Text = "„·Ê‰";
            // 
            // chkDuplex
            // 
            chkDuplex.AutoSize = true;
            chkDuplex.Checked = true;
            chkDuplex.CheckState = CheckState.Checked;
            chkDuplex.Location = new Point(198, 92);
            chkDuplex.Margin = new Padding(4, 3, 4, 3);
            chkDuplex.Name = "chkDuplex";
            chkDuplex.Size = new Size(91, 19);
            chkDuplex.TabIndex = 9;
            chkDuplex.Text = "„”Õ «·ÊÃÂÌ‰";
            // 
            // txtDPI
            // 
            txtDPI.Location = new Point(198, 115);
            txtDPI.Margin = new Padding(4, 3, 4, 3);
            txtDPI.Name = "txtDPI";
            txtDPI.Size = new Size(58, 23);
            txtDPI.TabIndex = 11;
            txtDPI.Text = "150";
            // 
            // lblDPI
            // 
            lblDPI.AutoSize = true;
            lblDPI.Location = new Point(257, 119);
            lblDPI.Margin = new Padding(4, 0, 4, 0);
            lblDPI.Name = "lblDPI";
            lblDPI.Size = new Size(28, 15);
            lblDPI.TabIndex = 10;
            lblDPI.Text = "DPI:";
            // 
            // chkContinuousScan
            // 
            chkContinuousScan.AutoSize = true;
            chkContinuousScan.Location = new Point(303, 92);
            chkContinuousScan.Margin = new Padding(4, 3, 4, 3);
            chkContinuousScan.Name = "chkContinuousScan";
            chkContinuousScan.Size = new Size(86, 19);
            chkContinuousScan.TabIndex = 15;
            chkContinuousScan.Text = "„”Õ „” „—";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(140, 138);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(117, 35);
            btnRefresh.TabIndex = 18;
            btnRefresh.Text = " ÕœÌÀ";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // pnlSearch
            // 
            pnlSearch.Controls.Add(btnSaveToDatabase);
            pnlSearch.Controls.Add(lstUnidentified);
            pnlSearch.Controls.Add(webBrowserPdf);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(txtUniqueNumber);
            pnlSearch.Controls.Add(lblExpress);
            pnlSearch.Dock = DockStyle.Bottom;
            pnlSearch.Location = new Point(0, 409);
            pnlSearch.Margin = new Padding(4, 3, 4, 3);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Size = new Size(1050, 404);
            pnlSearch.TabIndex = 30;
            pnlSearch.Visible = false;
            // 
            // btnSaveToDatabase
            // 
            btnSaveToDatabase.Location = new Point(14, 14);
            btnSaveToDatabase.Margin = new Padding(4, 3, 4, 3);
            btnSaveToDatabase.Name = "btnSaveToDatabase";
            btnSaveToDatabase.Size = new Size(117, 27);
            btnSaveToDatabase.TabIndex = 26;
            btnSaveToDatabase.Text = "Õ›Ÿ";
            btnSaveToDatabase.UseVisualStyleBackColor = true;
            btnSaveToDatabase.Visible = false;
            btnSaveToDatabase.Click += btnSaveToDatabase_Click;
            // 
            // lstUnidentified
            // 
            lstUnidentified.FormattingEnabled = true;
            lstUnidentified.ItemHeight = 15;
            lstUnidentified.Location = new Point(677, 14);
            lstUnidentified.Margin = new Padding(4, 3, 4, 3);
            lstUnidentified.Name = "lstUnidentified";
            lstUnidentified.Size = new Size(349, 364);
            lstUnidentified.TabIndex = 27;
            lstUnidentified.Visible = false;
            lstUnidentified.SelectedIndexChanged += lstUnidentified_SelectedIndexChanged;
            // 
            // webBrowserPdf
            // 
            webBrowserPdf.Location = new Point(14, 46);
            webBrowserPdf.Margin = new Padding(4, 3, 4, 3);
            webBrowserPdf.MinimumSize = new Size(23, 23);
            webBrowserPdf.Name = "webBrowserPdf";
            webBrowserPdf.Size = new Size(642, 335);
            webBrowserPdf.TabIndex = 25;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(537, 12);
            btnSearch.Margin = new Padding(4, 3, 4, 3);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(117, 27);
            btnSearch.TabIndex = 22;
            btnSearch.Text = "»ÕÀ";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += BtnSearch_Click;
            // 
            // txtUniqueNumber
            // 
            txtUniqueNumber.Location = new Point(397, 14);
            txtUniqueNumber.Margin = new Padding(4, 3, 4, 3);
            txtUniqueNumber.Name = "txtUniqueNumber";
            txtUniqueNumber.Size = new Size(132, 23);
            txtUniqueNumber.TabIndex = 23;
            // 
            // lblExpress
            // 
            lblExpress.AutoSize = true;
            lblExpress.Location = new Point(327, 17);
            lblExpress.Margin = new Padding(4, 0, 4, 0);
            lblExpress.Name = "lblExpress";
            lblExpress.Size = new Size(65, 15);
            lblExpress.TabIndex = 24;
            lblExpress.Text = "—ﬁ„ «·„” ‰œ";
            // 
            // DocGroupBox
            // 
            DocGroupBox.Controls.Add(chkWithoutIdentity);
            DocGroupBox.Controls.Add(dtTime);
            DocGroupBox.Controls.Add(txtMissedNumbers);
            DocGroupBox.Controls.Add(txtBtwBranch);
            DocGroupBox.Controls.Add(cbBtwBranches);
            DocGroupBox.Controls.Add(lblDocStartNumber);
            DocGroupBox.Controls.Add(txtStartDocNumber);
            DocGroupBox.Controls.Add(cbBranches);
            DocGroupBox.Controls.Add(cbCashDocs);
            DocGroupBox.Location = new Point(516, 27);
            DocGroupBox.Margin = new Padding(4, 3, 4, 3);
            DocGroupBox.Name = "DocGroupBox";
            DocGroupBox.Padding = new Padding(4, 3, 4, 3);
            DocGroupBox.Size = new Size(534, 150);
            DocGroupBox.TabIndex = 28;
            DocGroupBox.TabStop = false;
            DocGroupBox.Text = "«⁄œ«œ«  «·„” ‰œ« ";
            // 
            // chkWithoutIdentity
            // 
            chkWithoutIdentity.AutoSize = true;
            chkWithoutIdentity.Location = new Point(7, 115);
            chkWithoutIdentity.Margin = new Padding(4, 3, 4, 3);
            chkWithoutIdentity.Name = "chkWithoutIdentity";
            chkWithoutIdentity.Size = new Size(78, 19);
            chkWithoutIdentity.TabIndex = 39;
            chkWithoutIdentity.Text = "»œÊ‰ ÂÊÌ…";
            // 
            // dtTime
            // 
            dtTime.Location = new Point(7, 23);
            dtTime.Margin = new Padding(4, 3, 4, 3);
            dtTime.Name = "dtTime";
            dtTime.Size = new Size(163, 23);
            dtTime.TabIndex = 38;
            // 
            // txtMissedNumbers
            // 
            txtMissedNumbers.Location = new Point(175, 81);
            txtMissedNumbers.Margin = new Padding(4, 3, 4, 3);
            txtMissedNumbers.Name = "txtMissedNumbers";
            txtMissedNumbers.PlaceholderText = "«œŒ· «·«—ﬁ«„ «·„›ﬁÊœ…";
            txtMissedNumbers.RightToLeft = RightToLeft.No;
            txtMissedNumbers.Size = new Size(163, 23);
            txtMissedNumbers.TabIndex = 36;
            txtMissedNumbers.Visible = false;
            txtMissedNumbers.Leave += txtMissedNumbers_Leave;
            // 
            // txtBtwBranch
            // 
            txtBtwBranch.Location = new Point(178, 54);
            txtBtwBranch.Margin = new Padding(4, 3, 4, 3);
            txtBtwBranch.Name = "txtBtwBranch";
            txtBtwBranch.Size = new Size(116, 23);
            txtBtwBranch.TabIndex = 34;
            txtBtwBranch.Text = "1";
            txtBtwBranch.Visible = false;
            txtBtwBranch.TextChanged += txtStartDocNumber_TextChanged;
            // 
            // cbBtwBranches
            // 
            cbBtwBranches.AutoSize = true;
            cbBtwBranches.Location = new Point(350, 54);
            cbBtwBranches.Margin = new Padding(4, 3, 4, 3);
            cbBtwBranches.Name = "cbBtwBranches";
            cbBtwBranches.Size = new Size(142, 19);
            cbBtwBranches.TabIndex = 33;
            cbBtwBranches.Text = "”‰œ«  ’—› »Ì‰ «·›—Ê⁄";
            cbBtwBranches.UseVisualStyleBackColor = true;
            cbBtwBranches.CheckedChanged += cbBtwBranches_CheckedChanged;
            // 
            // lblDocStartNumber
            // 
            lblDocStartNumber.AutoSize = true;
            lblDocStartNumber.Location = new Point(178, 8);
            lblDocStartNumber.Margin = new Padding(4, 0, 4, 0);
            lblDocStartNumber.Name = "lblDocStartNumber";
            lblDocStartNumber.Size = new Size(105, 15);
            lblDocStartNumber.TabIndex = 32;
            lblDocStartNumber.Text = "—ﬁ„ »œ«Ì… «·„” ‰œ« ";
            lblDocStartNumber.Visible = false;
            // 
            // txtStartDocNumber
            // 
            txtStartDocNumber.Location = new Point(178, 24);
            txtStartDocNumber.Margin = new Padding(4, 3, 4, 3);
            txtStartDocNumber.Name = "txtStartDocNumber";
            txtStartDocNumber.Size = new Size(116, 23);
            txtStartDocNumber.TabIndex = 31;
            txtStartDocNumber.Text = "1";
            txtStartDocNumber.Visible = false;
            txtStartDocNumber.TextChanged += txtStartDocNumber_TextChanged;
            // 
            // cbBranches
            // 
            cbBranches.DropDownStyle = ComboBoxStyle.DropDownList;
            cbBranches.FormattingEnabled = true;
            cbBranches.Location = new Point(350, 23);
            cbBranches.Margin = new Padding(4, 3, 4, 3);
            cbBranches.Name = "cbBranches";
            cbBranches.Size = new Size(168, 23);
            cbBranches.TabIndex = 30;
            cbBranches.SelectedIndexChanged += cbBranches_SelectedIndexChanged;
            // 
            // cbCashDocs
            // 
            cbCashDocs.AutoSize = true;
            cbCashDocs.Location = new Point(350, 81);
            cbCashDocs.Margin = new Padding(4, 3, 4, 3);
            cbCashDocs.Name = "cbCashDocs";
            cbCashDocs.Size = new Size(118, 19);
            cbCashDocs.TabIndex = 29;
            cbCashDocs.Text = "”‰œ«  ’—› ‰ﬁœÌ";
            cbCashDocs.UseVisualStyleBackColor = true;
            cbCashDocs.CheckedChanged += cbCashDocs_CheckedChanged;
            // 
            // BtnGetFile
            // 
            BtnGetFile.BackColor = SystemColors.ButtonFace;
            BtnGetFile.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            BtnGetFile.Location = new Point(494, 183);
            BtnGetFile.Margin = new Padding(4, 3, 4, 3);
            BtnGetFile.Name = "BtnGetFile";
            BtnGetFile.Size = new Size(175, 58);
            BtnGetFile.TabIndex = 27;
            BtnGetFile.Text = "«Œ — „·› ·Õ›ŸÂ";
            BtnGetFile.UseVisualStyleBackColor = false;
            BtnGetFile.Visible = false;
            BtnGetFile.Click += BtnGetFile_Click;
            // 
            // btnGetAllExpress
            // 
            btnGetAllExpress.Location = new Point(680, 183);
            btnGetAllExpress.Margin = new Padding(4, 3, 4, 3);
            btnGetAllExpress.Name = "btnGetAllExpress";
            btnGetAllExpress.Size = new Size(140, 27);
            btnGetAllExpress.TabIndex = 28;
            btnGetAllExpress.Text = "Õ›Ÿ Ã„Ì⁄ «·„·›« ";
            btnGetAllExpress.UseVisualStyleBackColor = true;
            btnGetAllExpress.Visible = false;
            btnGetAllExpress.Click += btnGetAllExpress_Click;
            // 
            // BtnRandomSave
            // 
            BtnRandomSave.Location = new Point(828, 181);
            BtnRandomSave.Margin = new Padding(4, 3, 4, 3);
            BtnRandomSave.Name = "BtnRandomSave";
            BtnRandomSave.Size = new Size(140, 27);
            BtnRandomSave.TabIndex = 32;
            BtnRandomSave.Text = "Õ›Ÿ ⁄‘Ê«∆Ì";
            BtnRandomSave.UseVisualStyleBackColor = true;
            BtnRandomSave.Visible = false;
            BtnRandomSave.Click += BtnRandomSave_Click;
            // 
            // BtnGet1Express
            // 
            BtnGet1Express.Location = new Point(828, 214);
            BtnGet1Express.Margin = new Padding(4, 3, 4, 3);
            BtnGet1Express.Name = "BtnGet1Express";
            BtnGet1Express.Size = new Size(140, 27);
            BtnGet1Express.TabIndex = 29;
            BtnGet1Express.Text = "Ã·» «·«ﬂ”»—”";
            BtnGet1Express.UseVisualStyleBackColor = true;
            BtnGet1Express.Visible = false;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { BtnEditDeleteLastScanned, BtnSearchDoc, BtnUploadFile, tsmiSettings });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1050, 24);
            menuStrip1.TabIndex = 29;
            menuStrip1.Text = "menuStrip1";
            // 
            // BtnEditDeleteLastScanned
            // 
            BtnEditDeleteLastScanned.Name = "BtnEditDeleteLastScanned";
            BtnEditDeleteLastScanned.Size = new Size(177, 20);
            BtnEditDeleteLastScanned.Text = " ⁄œÌ· «Ê Õ–› «·„” ‰œ«  «·«ŒÌ—…";
            BtnEditDeleteLastScanned.Click += BtnEditDeleteLastScanned_Click;
            // 
            // BtnSearchDoc
            // 
            BtnSearchDoc.Name = "BtnSearchDoc";
            BtnSearchDoc.Size = new Size(100, 20);
            BtnSearchDoc.Text = "«·»ÕÀ ⁄‰ „” ‰œ";
            BtnSearchDoc.Click += BtnSearch_Click;
            // 
            // BtnUploadFile
            // 
            BtnUploadFile.Name = "BtnUploadFile";
            BtnUploadFile.Size = new Size(75, 20);
            BtnUploadFile.Text = " Õ„Ì· „·›";
            BtnUploadFile.Click += BtnUploadFile_Click;
            // 
            // tsmiSettings
            // 
            tsmiSettings.Name = "tsmiSettings";
            tsmiSettings.Size = new Size(58, 20);
            tsmiSettings.Text = "«⁄œ«œ« ";
            tsmiSettings.Click += tsmiSettings_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statuslbl1, statuslbl2 });
            statusStrip1.Location = new Point(0, 387);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(16, 0, 1, 0);
            statusStrip1.Size = new Size(1050, 22);
            statusStrip1.TabIndex = 31;
            // 
            // statuslbl1
            // 
            statuslbl1.Name = "statuslbl1";
            statuslbl1.Size = new Size(30, 17);
            statuslbl1.Text = "Ã«Â“";
            // 
            // statuslbl2
            // 
            statuslbl2.Name = "statuslbl2";
            statuslbl2.Size = new Size(0, 17);
            // 
            // btnTestQueue
            // 
            btnTestQueue.Location = new Point(412, 187);
            btnTestQueue.Name = "btnTestQueue";
            btnTestQueue.Size = new Size(75, 23);
            btnTestQueue.TabIndex = 33;
            btnTestQueue.Text = "button1";
            btnTestQueue.UseVisualStyleBackColor = true;
            btnTestQueue.Click += btnTestQueue_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 813);
            Controls.Add(btnTestQueue);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(btnGetAllExpress);
            Controls.Add(BtnRandomSave);
            Controls.Add(BtnGet1Express);
            Controls.Add(BtnGetFile);
            Controls.Add(DocGroupBox);
            Controls.Add(pnlSearch);
            Controls.Add(btnRefresh);
            Controls.Add(chkContinuousScan);
            Controls.Add(lblDPI);
            Controls.Add(txtDPI);
            Controls.Add(chkDuplex);
            Controls.Add(groupBox1);
            Controls.Add(lblErrors);
            Controls.Add(lblInfo);
            Controls.Add(progressBar1);
            Controls.Add(btnCancel);
            Controls.Add(btnScan);
            Controls.Add(btnRefreshScanners);
            Controls.Add(cboScanners);
            Controls.Add(lblStatus);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            RightToLeft = RightToLeft.Yes;
            Text = "‰Ÿ«„ √—‘›… «·„” ‰œ« ";
            FormClosing += OnFormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            DocGroupBox.ResumeLayout(false);
            DocGroupBox.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboScanners;
        private System.Windows.Forms.Button btnRefreshScanners;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbBlackWhite;
        private System.Windows.Forms.RadioButton rbGrayscale;
        private System.Windows.Forms.RadioButton rbColor;
        private System.Windows.Forms.CheckBox chkDuplex;
        private System.Windows.Forms.TextBox txtDPI;
        private System.Windows.Forms.Label lblDPI;
        private System.Windows.Forms.CheckBox chkContinuousScan;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Button btnSaveToDatabase;
        private System.Windows.Forms.ListBox lstUnidentified;
        private System.Windows.Forms.WebBrowser webBrowserPdf;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtUniqueNumber;
        private System.Windows.Forms.Label lblExpress;
        private System.Windows.Forms.GroupBox DocGroupBox;
        private System.Windows.Forms.CheckBox chkWithoutIdentity;
        private System.Windows.Forms.DateTimePicker dtTime;
        private System.Windows.Forms.TextBox txtMissedNumbers;
        private System.Windows.Forms.TextBox txtBtwBranch;
        private System.Windows.Forms.CheckBox cbBtwBranches;
        private System.Windows.Forms.Label lblDocStartNumber;
        private System.Windows.Forms.TextBox txtStartDocNumber;
        private System.Windows.Forms.ComboBox cbBranches;
        private System.Windows.Forms.CheckBox cbCashDocs;
        private System.Windows.Forms.Button BtnGetFile;
        private System.Windows.Forms.Button btnGetAllExpress;
        private System.Windows.Forms.Button BtnRandomSave;
        private System.Windows.Forms.Button BtnGet1Express;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BtnEditDeleteLastScanned;
        private System.Windows.Forms.ToolStripMenuItem BtnSearchDoc;
        private System.Windows.Forms.ToolStripMenuItem BtnUploadFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl1;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl2;
        private ToolStripMenuItem tsmiSettings;
        private Button btnTestQueue;
    }
}