namespace DocumentArchiever.UI.Forms
{
    partial class SearchForm
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
            cbBranches = new ComboBox();
            lblExpress = new Label();
            btnSearch = new Button();
            txtUniqueNumber = new TextBox();
            lblErrors = new Label();
            lblInfo = new Label();
            IsCashPayment = new CheckBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBox1 = new GroupBox();
            cmbYears = new ComboBox();
            cbBtwBranches = new CheckBox();
            webBrowserPdf = new WebBrowser();
            tableLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // cbBranches
            // 
            cbBranches.Dock = DockStyle.Fill;
            cbBranches.DropDownStyle = ComboBoxStyle.DropDownList;
            cbBranches.FormattingEnabled = true;
            cbBranches.Location = new Point(4, 188);
            cbBranches.Margin = new Padding(4, 3, 4, 3);
            cbBranches.Name = "cbBranches";
            cbBranches.Size = new Size(255, 23);
            cbBranches.TabIndex = 31;
            // 
            // lblExpress
            // 
            lblExpress.AutoSize = true;
            lblExpress.Location = new Point(166, 18);
            lblExpress.Margin = new Padding(4, 0, 4, 0);
            lblExpress.Name = "lblExpress";
            lblExpress.Size = new Size(65, 15);
            lblExpress.TabIndex = 24;
            lblExpress.Text = "—ﬁ„ «·„” ‰œ";
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(10, 45);
            btnSearch.Margin = new Padding(4, 3, 4, 3);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(231, 27);
            btnSearch.TabIndex = 22;
            btnSearch.Text = "»ÕÀ";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += BtnSearch_Click;
            // 
            // txtUniqueNumber
            // 
            txtUniqueNumber.Location = new Point(0, 15);
            txtUniqueNumber.Margin = new Padding(4, 3, 4, 3);
            txtUniqueNumber.Name = "txtUniqueNumber";
            txtUniqueNumber.Size = new Size(158, 23);
            txtUniqueNumber.TabIndex = 23;
            // 
            // lblErrors
            // 
            lblErrors.Dock = DockStyle.Top;
            lblErrors.ForeColor = Color.Red;
            lblErrors.Location = new Point(267, 0);
            lblErrors.Margin = new Padding(4, 0, 4, 0);
            lblErrors.Name = "lblErrors";
            lblErrors.Size = new Size(779, 69);
            lblErrors.TabIndex = 28;
            lblErrors.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblInfo
            // 
            lblInfo.Dock = DockStyle.Bottom;
            lblInfo.Location = new Point(0, 554);
            lblInfo.Margin = new Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1050, 23);
            lblInfo.TabIndex = 27;
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // IsCashPayment
            // 
            IsCashPayment.AutoSize = true;
            IsCashPayment.Location = new Point(74, 72);
            IsCashPayment.Margin = new Padding(4, 3, 4, 3);
            IsCashPayment.Name = "IsCashPayment";
            IsCashPayment.Size = new Size(104, 19);
            IsCashPayment.TabIndex = 29;
            IsCashPayment.Text = "”‰œ ’—› ‰ﬁœÌ";
            IsCashPayment.UseVisualStyleBackColor = true;
            IsCashPayment.CheckedChanged += IsCashPayment_CheckedChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(cbBranches, 1, 1);
            tableLayoutPanel1.Controls.Add(groupBox1, 1, 0);
            tableLayoutPanel1.Controls.Add(webBrowserPdf, 0, 1);
            tableLayoutPanel1.Controls.Add(lblErrors, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.Size = new Size(1050, 554);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cmbYears);
            groupBox1.Controls.Add(cbBtwBranches);
            groupBox1.Controls.Add(lblExpress);
            groupBox1.Controls.Add(IsCashPayment);
            groupBox1.Controls.Add(txtUniqueNumber);
            groupBox1.Controls.Add(btnSearch);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(4, 3);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(255, 179);
            groupBox1.TabIndex = 28;
            groupBox1.TabStop = false;
            groupBox1.Text = "„⁄«ÌÌ— «·»ÕÀ";
            // 
            // cmbYears
            // 
            cmbYears.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbYears.FormattingEnabled = true;
            cmbYears.Location = new Point(62, 129);
            cmbYears.Margin = new Padding(4, 3, 4, 3);
            cmbYears.Name = "cmbYears";
            cmbYears.Size = new Size(140, 23);
            cmbYears.TabIndex = 35;
            // 
            // cbBtwBranches
            // 
            cbBtwBranches.AutoSize = true;
            cbBtwBranches.Location = new Point(74, 98);
            cbBtwBranches.Margin = new Padding(4, 3, 4, 3);
            cbBtwBranches.Name = "cbBtwBranches";
            cbBtwBranches.Size = new Size(142, 19);
            cbBtwBranches.TabIndex = 34;
            cbBtwBranches.Text = "”‰œ«  ’—› »Ì‰ «·›—Ê⁄";
            cbBtwBranches.UseVisualStyleBackColor = true;
            cbBtwBranches.CheckedChanged += cbBtwBranches_CheckedChanged;
            // 
            // webBrowserPdf
            // 
            webBrowserPdf.Dock = DockStyle.Fill;
            webBrowserPdf.Location = new Point(267, 188);
            webBrowserPdf.Margin = new Padding(4, 3, 4, 3);
            webBrowserPdf.MinimumSize = new Size(23, 23);
            webBrowserPdf.Name = "webBrowserPdf";
            webBrowserPdf.Size = new Size(779, 339);
            webBrowserPdf.TabIndex = 27;
            // 
            // SearchForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 577);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(lblInfo);
            Margin = new Padding(4, 3, 4, 3);
            Name = "SearchForm";
            RightToLeft = RightToLeft.Yes;
            Text = "«·»ÕÀ ⁄‰ „” ‰œ";
            tableLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.ComboBox cbBranches;
        private System.Windows.Forms.Label lblExpress;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtUniqueNumber;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.CheckBox IsCashPayment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbBtwBranches;
        private System.Windows.Forms.WebBrowser webBrowserPdf;
        private System.Windows.Forms.ComboBox cmbYears;
    }
}