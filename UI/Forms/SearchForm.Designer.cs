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
            this.cbBranches = new System.Windows.Forms.ComboBox();
            this.lblExpress = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtUniqueNumber = new System.Windows.Forms.TextBox();
            this.lblErrors = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.IsCashPayment = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBtwBranches = new System.Windows.Forms.CheckBox();
            this.webBrowserPdf = new System.Windows.Forms.WebBrowser();
            this.cmbYears = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();

            // tableLayoutPanel1
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.cbBranches, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.webBrowserPdf, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblErrors, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(900, 500);
            this.tableLayoutPanel1.TabIndex = 0;

            // cbBranches
            this.cbBranches.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBranches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbBranches.FormattingEnabled = true;
            this.cbBranches.Location = new System.Drawing.Point(678, 171);
            this.cbBranches.Name = "cbBranches";
            this.cbBranches.Size = new System.Drawing.Size(219, 21);
            this.cbBranches.TabIndex = 31;

            // lblExpress
            this.lblExpress.AutoSize = true;
            this.lblExpress.Location = new System.Drawing.Point(142, 16);
            this.lblExpress.Name = "lblExpress";
            this.lblExpress.Size = new System.Drawing.Size(76, 13);
            this.lblExpress.TabIndex = 24;
            this.lblExpress.Text = "—ﬁ„ «·„” ‰œ";

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(9, 39);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(198, 23);
            this.btnSearch.TabIndex = 22;
            this.btnSearch.Text = "»ÕÀ";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);

            // txtUniqueNumber
            this.txtUniqueNumber.Location = new System.Drawing.Point(0, 13);
            this.txtUniqueNumber.Name = "txtUniqueNumber";
            this.txtUniqueNumber.Size = new System.Drawing.Size(136, 20);
            this.txtUniqueNumber.TabIndex = 23;

            // lblErrors
            this.lblErrors.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblErrors.ForeColor = System.Drawing.Color.Red;
            this.lblErrors.Location = new System.Drawing.Point(3, 0);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(669, 60);
            this.lblErrors.TabIndex = 28;
            this.lblErrors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblInfo
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblInfo.Location = new System.Drawing.Point(0, 480);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(900, 20);
            this.lblInfo.TabIndex = 27;
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // IsCashPayment
            this.IsCashPayment.AutoSize = true;
            this.IsCashPayment.Location = new System.Drawing.Point(63, 62);
            this.IsCashPayment.Name = "IsCashPayment";
            this.IsCashPayment.Size = new System.Drawing.Size(102, 17);
            this.IsCashPayment.TabIndex = 29;
            this.IsCashPayment.Text = "”‰œ ’—› ‰ﬁœÌ";
            this.IsCashPayment.UseVisualStyleBackColor = true;
            this.IsCashPayment.CheckedChanged += new System.EventHandler(this.IsCashPayment_CheckedChanged);

            // groupBox1
            this.groupBox1.Controls.Add(this.cmbYears);
            this.groupBox1.Controls.Add(this.cbBtwBranches);
            this.groupBox1.Controls.Add(this.lblExpress);
            this.groupBox1.Controls.Add(this.IsCashPayment);
            this.groupBox1.Controls.Add(this.txtUniqueNumber);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(678, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 162);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "„⁄«ÌÌ— «·»ÕÀ";

            // cbBtwBranches
            this.cbBtwBranches.AutoSize = true;
            this.cbBtwBranches.Location = new System.Drawing.Point(63, 85);
            this.cbBtwBranches.Name = "cbBtwBranches";
            this.cbBtwBranches.Size = new System.Drawing.Size(145, 17);
            this.cbBtwBranches.TabIndex = 34;
            this.cbBtwBranches.Text = "”‰œ«  ’—› »Ì‰ «·›—Ê⁄";
            this.cbBtwBranches.UseVisualStyleBackColor = true;
            this.cbBtwBranches.CheckedChanged += new System.EventHandler(this.cbBtwBranches_CheckedChanged);

            // webBrowserPdf
            this.webBrowserPdf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserPdf.Location = new System.Drawing.Point(3, 171);
            this.webBrowserPdf.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPdf.Name = "webBrowserPdf";
            this.webBrowserPdf.Size = new System.Drawing.Size(669, 306);
            this.webBrowserPdf.TabIndex = 27;

            // cmbYears
            this.cmbYears.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYears.FormattingEnabled = true;
            this.cmbYears.Location = new System.Drawing.Point(53, 112);
            this.cmbYears.Name = "cmbYears";
            this.cmbYears.Size = new System.Drawing.Size(121, 21);
            this.cmbYears.TabIndex = 35;

            // SearchForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblInfo);
            this.Name = "SearchForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "«·»ÕÀ ⁄‰ „” ‰œ";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
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