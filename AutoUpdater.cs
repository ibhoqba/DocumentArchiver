using DocumentArchiever.Services;
using System.Diagnostics;
using System.Reflection;

namespace DocumentArchiever
{
    public class AutoUpdater
    {
        private const string VersionCheckUrl = "http://balco.ddns.net/app/version.txt";
        private const string DownloadUrlExe = "http://balco.ddns.net/app/DocumentArchiever.exe";
        private const string CurrentExeName = "DocumentArchiever.exe";

        private readonly ILogger _logger;

        public AutoUpdater(ILogger logger)
        {
            _logger = logger;
        }

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                string latestVersionString = await DownloadTextFileAsync(VersionCheckUrl);

                if (!string.IsNullOrEmpty(latestVersionString) && Version.TryParse(latestVersionString.Trim(), out Version latestVersion))
                {
                    if (latestVersion > currentVersion)
                    {
                        DialogResult result = MessageBox.Show(
                            $"New update available ({latestVersion}). Do you want to download the update?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            await DownloadAndReplaceExeAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for updates");
                MessageBox.Show($"An error occurred during the update check: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> DownloadTextFileAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException)
                {
                    return null;
                }
            }
        }

        private async Task DownloadAndReplaceExeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string tempExePath = Path.Combine(Path.GetTempPath(), "New_" + CurrentExeName);

                    using (var stream = await client.GetStreamAsync(DownloadUrlExe))
                    using (var fileStream = new FileStream(tempExePath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    string currentExePath = Assembly.GetEntryAssembly().Location;
                    string backupPath = currentExePath + ".backup_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    try
                    {
                        File.Move(currentExePath, backupPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Backup failed: {ex.Message}");
                    }

                    try
                    {
                        File.Move(tempExePath, currentExePath);
                        MessageBox.Show("Application updated successfully. It will now restart.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.Start(Assembly.GetEntryAssembly().Location);
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error replacing the application file: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (File.Exists(backupPath))
                        {
                            try
                            {
                                File.Move(backupPath, currentExePath);
                            }
                            catch (Exception restoreEx)
                            {
                                _logger.LogError(restoreEx, "Failed to restore backup");
                            }
                        }
                    }
                    finally
                    {
                        if (File.Exists(tempExePath))
                        {
                            try { File.Delete(tempExePath); } catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error downloading update");
                    MessageBox.Show($"Error downloading the new application file: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}