namespace DocumentArchiever.Setting
{
    public partial class fmDbSettings : Form
    {
        public fmDbSettings()
        {
            InitializeComponent();
            txtServer.Text = Properties.Settings.Default.dbServer;
            txtDbName.Text = Properties.Settings.Default.Database;
            txtUserName.Text = Properties.Settings.Default.DbUserId;
            txtPwd.Text = Properties.Settings.Default.DbUserPwd;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.dbServer = txtServer.Text.Trim();
            Properties.Settings.Default.Database = txtDbName.Text.Trim();
            Properties.Settings.Default.DbUserId = txtUserName.Text.Trim();
            Properties.Settings.Default.DbUserPwd = txtPwd.Text.Trim();

            // Persist changes
            Properties.Settings.Default.Save();

            MessageBox.Show("تم حفظ بيانات الاتصال بنجاح");
            this.Close();

        }
    }
}
