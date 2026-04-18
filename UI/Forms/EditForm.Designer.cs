namespace DocumentArchiever.UI.Forms
{
    partial class EditForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtDocNum = new System.Windows.Forms.TextBox();
            this.LblError = new System.Windows.Forms.Label();
            this.BtnDelSelectedDocs = new System.Windows.Forms.Button();
            this.BtnGetLastStoredDocs = new System.Windows.Forms.Button();
            this.LblLastSavedDocs = new System.Windows.Forms.Label();
            this.TxtLastSavedDocs = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            // tableLayoutPanel1
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;

            // groupBox1
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtDocNum);
            this.groupBox1.Controls.Add(this.LblError);
            this.groupBox1.Controls.Add(this.BtnDelSelectedDocs);
            this.groupBox1.Controls.Add(this.BtnGetLastStoredDocs);
            this.groupBox1.Controls.Add(this.LblLastSavedDocs);
            this.groupBox1.Controls.Add(this.TxtLastSavedDocs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(794, 94);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "«·⁄„·Ì« ";

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(672, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "—ﬁ„ «·„” ‰œ «·„—«œ";

            // TxtDocNum
            this.TxtDocNum.Location = new System.Drawing.Point(647, 80);
            this.TxtDocNum.Name = "TxtDocNum";
            this.TxtDocNum.Size = new System.Drawing.Size(135, 20);
            this.TxtDocNum.TabIndex = 6;
            this.TxtDocNum.Enter += new System.EventHandler(this.TxtDocNum_Enter);
            this.TxtDocNum.Leave += new System.EventHandler(this.TxtDocNum_Leave);

            // LblError
            this.LblError.AutoSize = true;
            this.LblError.Location = new System.Drawing.Point(333, 73);
            this.LblError.Name = "LblError";
            this.LblError.Size = new System.Drawing.Size(0, 13);
            this.LblError.TabIndex = 5;

            // BtnDelSelectedDocs
            this.BtnDelSelectedDocs.Location = new System.Drawing.Point(176, 47);
            this.BtnDelSelectedDocs.Name = "BtnDelSelectedDocs";
            this.BtnDelSelectedDocs.Size = new System.Drawing.Size(167, 23);
            this.BtnDelSelectedDocs.TabIndex = 4;
            this.BtnDelSelectedDocs.Text = "Õ–› «·„” ‰œ«  «·„Œ «—…";
            this.BtnDelSelectedDocs.UseVisualStyleBackColor = true;
            this.BtnDelSelectedDocs.Visible = false;
            this.BtnDelSelectedDocs.Click += new System.EventHandler(this.BtnDelSelectedDocs_Click);

            // BtnGetLastStoredDocs
            this.BtnGetLastStoredDocs.Location = new System.Drawing.Point(438, 48);
            this.BtnGetLastStoredDocs.Name = "BtnGetLastStoredDocs";
            this.BtnGetLastStoredDocs.Size = new System.Drawing.Size(167, 23);
            this.BtnGetLastStoredDocs.TabIndex = 3;
            this.BtnGetLastStoredDocs.Text = "Ã·» «Œ— «·„” ‰œ«  «·„Õ›ÊŸ…";
            this.BtnGetLastStoredDocs.UseVisualStyleBackColor = true;
            this.BtnGetLastStoredDocs.Click += new System.EventHandler(this.BtnGetLastStoredDocs_Click);

            // LblLastSavedDocs
            this.LblLastSavedDocs.AutoSize = true;
            this.LblLastSavedDocs.Location = new System.Drawing.Point(647, 16);
            this.LblLastSavedDocs.Name = "LblLastSavedDocs";
            this.LblLastSavedDocs.Size = new System.Drawing.Size(138, 13);
            this.LblLastSavedDocs.TabIndex = 2;
            this.LblLastSavedDocs.Text = "⁄œœ «·„” ‰œ«  «·„—«œ Õ–›Â«";

            // TxtLastSavedDocs
            this.TxtLastSavedDocs.Location = new System.Drawing.Point(647, 32);
            this.TxtLastSavedDocs.Name = "TxtLastSavedDocs";
            this.TxtLastSavedDocs.Size = new System.Drawing.Size(135, 20);
            this.TxtLastSavedDocs.TabIndex = 0;
            this.TxtLastSavedDocs.Text = "10";
            this.TxtLastSavedDocs.Enter += new System.EventHandler(this.TxtLastSavedDocs_Enter);
            this.TxtLastSavedDocs.Leave += new System.EventHandler(this.TxtLastSavedDocs_Leave);

            // dataGridView1
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 103);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(794, 344);
            this.dataGridView1.TabIndex = 1;

            // EditForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EditForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = " ⁄œÌ· ÊÕ–› «·„” ‰œ« ";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtDocNum;
        private System.Windows.Forms.Label LblError;
        private System.Windows.Forms.Button BtnDelSelectedDocs;
        private System.Windows.Forms.Button BtnGetLastStoredDocs;
        private System.Windows.Forms.Label LblLastSavedDocs;
        private System.Windows.Forms.TextBox TxtLastSavedDocs;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}