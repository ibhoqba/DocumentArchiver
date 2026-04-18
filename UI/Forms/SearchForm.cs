using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentArchiever.Data;
using DocumentArchiever.Data.Entities;
using DocumentArchiever.Services;

namespace DocumentArchiever.UI.Forms
{
    public partial class SearchForm : Form
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger _logger;
        private List<Branch> _branches;

        public SearchForm(IDatabaseService databaseService, ILogger logger)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _logger = logger;
            Load += OnFormLoad;
        }

        private async void OnFormLoad(object sender, EventArgs e)
        {
            await LoadBranchesAsync();
            await LoadYearsAsync();
        }

        private async Task LoadBranchesAsync()
        {
            try
            {
                _branches = await _databaseService.GetBranchesAsync();
                cbBranches.DataSource = _branches;
                cbBranches.DisplayMember = "Name";
                cbBranches.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load branches in search form");
                MessageBox.Show($"Œÿ√ ›Ì  Õ„Ì· «·›—Ê⁄: {ex.Message}", "Œÿ√",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadYearsAsync()
        {
            try
            {
                var years = await _databaseService.GetYearsAsync();
                cmbYears.DataSource = years;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load years in search form");
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUniqueNumber.Text))
            {
                lblInfo.Text = "Ì—ÃÏ ≈œŒ«· —ﬁ„ «·„” ‰œ";
                return;
            }

            string searchNumber = txtUniqueNumber.Text.Trim();

            if (!searchNumber.All(char.IsDigit))
            {
                lblInfo.Text = "Ì—ÃÏ ≈œŒ«· √—ﬁ«„ ›ﬁÿ";
                return;
            }

            try
            {
                int? branchId = null;
                int? docType = null;

                if (!searchNumber.StartsWith($"{Core.OCR.TesseractOcrEngine.ExpressNumber}"))
                {
                    if (cbBranches.SelectedItem == null)
                    {
                        lblInfo.Text = "Ì—ÃÏ «Œ Ì«— «·›—⁄";
                        return;
                    }

                    branchId = (int)cbBranches.SelectedValue;

                    if (IsCashPayment.Checked)
                        docType = 18;
                    else if (cbBtwBranches.Checked)
                        docType = 91;
                }

                int year = cmbYears.SelectedItem != null ? (int)cmbYears.SelectedItem : DateTime.Now.Year;

                Document document = null;

                if (branchId.HasValue && docType.HasValue)
                {
                    document = await _databaseService.GetDocumentAsync(searchNumber, branchId.Value);
                }
                else
                {
                    // Search all branches
                    var branches = await _databaseService.GetBranchesAsync();
                    foreach (var branch in branches)
                    {
                        document = await _databaseService.GetDocumentAsync(searchNumber, branch.Id);
                        if (document != null) break;
                    }
                }

                if (document != null)
                {
                    string fullPath = Path.Combine(
                        Properties.Settings.Default.SavePath,
                        document.FilePath,
                        document.FileName);

                    if (File.Exists(fullPath))
                    {
                        webBrowserPdf.Navigate(fullPath);
                        lblInfo.Text = $" „ «·⁄ÀÊ— ⁄·Ï «·„” ‰œ {searchNumber}";
                    }
                    else
                    {
                        lblInfo.Text = $"«·„·› €Ì— „ÊÃÊœ ⁄·Ï «·ﬁ—’: {fullPath}";
                    }
                }
                else
                {
                    webBrowserPdf.Navigate("about:blank");
                    lblInfo.Text = $"·„ Ì „ «·⁄ÀÊ— ⁄·Ï „” ‰œ »—ﬁ„ {searchNumber}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search failed");
                lblInfo.Text = $"Œÿ√ ›Ì «·»ÕÀ: {ex.Message}";
            }
        }

        private void IsCashPayment_CheckedChanged(object sender, EventArgs e)
        {
            if (IsCashPayment.Checked)
            {
                cbBtwBranches.Checked = false;
            }
        }

        private void cbBtwBranches_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBtwBranches.Checked)
            {
                IsCashPayment.Checked = false;
            }
        }
    }
}