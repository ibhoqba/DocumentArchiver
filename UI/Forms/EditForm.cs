using DocumentArchiever.Data;
using DocumentArchiever.Data.Entities;
using DocumentArchiever.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DocumentArchiever.UI.Forms
{
    public partial class EditForm : Form
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger _logger;
        private List<Document> _documents;

        public EditForm(IDatabaseService databaseService, ILogger logger)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _logger = logger;
            Load += OnFormLoad;
        }

        private async void OnFormLoad(object sender, EventArgs e)
        {
            await LoadRecentDocumentsAsync();
        }

        private async Task LoadRecentDocumentsAsync()
        {
            try
            {
                int count = int.TryParse(TxtLastSavedDocs.Text, out int c) ? c : 10;
                _documents = await _databaseService.GetRecentDocumentsAsync(count);

                dataGridView1.DataSource = _documents;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load recent documents");
                MessageBox.Show($"Œÿ√ ›Ì  Õ„Ì· «·„” ‰œ« : {ex.Message}", "Œÿ√",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnGetLastStoredDocs_Click(object sender, EventArgs e)
        {
            await LoadRecentDocumentsAsync();
            BtnDelSelectedDocs.Visible = true;
        }

        private async void BtnDelSelectedDocs_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("·«  ÊÃœ ’›Ê› „Õœœ… ··Õ–›", " ‰»ÌÂ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Â· √‰  „ √ﬂœ „‰ Õ–› {dataGridView1.SelectedRows.Count} „” ‰œø",
                " √ﬂÌœ «·Õ–›", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                var idsToDelete = new List<int>();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.DataBoundItem is Document doc)
                    {
                        idsToDelete.Add(doc.Id);
                    }
                }

                bool success = await _databaseService.DeleteDocumentsAsync(idsToDelete);

                if (success)
                {
                    await LoadRecentDocumentsAsync();
                    LblError.Text = " „ Õ–› «·„” ‰œ«  »‰Ã«Õ";
                }
                else
                {
                    LblError.Text = "›‘· Õ–› «·„” ‰œ« ";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete documents");
                MessageBox.Show($"Œÿ√ ›Ì «·Õ–›: {ex.Message}", "Œÿ√",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtLastSavedDocs_Enter(object sender, EventArgs e)
        {
            TxtDocNum.Enabled = false;
        }

        private void TxtDocNum_Enter(object sender, EventArgs e)
        {
            TxtLastSavedDocs.Enabled = false;
        }

        private void TxtDocNum_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtDocNum.Text))
                TxtLastSavedDocs.Enabled = true;
        }

        private void TxtLastSavedDocs_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtLastSavedDocs.Text))
                TxtDocNum.Enabled = true;
        }
    }
}