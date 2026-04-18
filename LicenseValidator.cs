using Microsoft.Win32;

namespace DocumentArchiever
{
    public static class LicenseValidator
    {
        private const string RegistryKeyPath = @"SOFTWARE\DocumentArchiever\OCRCore";
        private const string FirstRunValueName = "FirstRunDate";

        public static bool IsValid()
        {
            DateTime officialStartDate = new DateTime(2026, 4, 5);

            if (DateTime.Now.Date < officialStartDate)
            {
                MessageBox.Show(
                    $"This application will be available starting from {officialStartDate:d}.",
                    "Application Not Yet Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            DateTime firstRunDate = GetFirstRunDate(officialStartDate);

            if (DateTime.Now.Date > firstRunDate.AddMonths(3))
            {
                MessageBox.Show(
                    "Your 3-month trial period has expired.\n\nPlease purchase a license to continue using this software.",
                    "License Expired",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                return false;
            }

            int daysRemaining = (firstRunDate.AddMonths(3) - DateTime.Now.Date).Days;
            if (daysRemaining <= 5 && daysRemaining >= 0)
            {
                MessageBox.Show(
                    $"Your trial will expire in {daysRemaining} day{(daysRemaining != 1 ? "s" : "")}.\n\nPlease renew your license soon.",
                    "Trial Expiring Soon",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return true;
        }

        private static DateTime GetFirstRunDate(DateTime officialStartDate)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
            {
                if (key.GetValue(FirstRunValueName) == null)
                {
                    DateTime startDate = DateTime.Now.Date;
                    key.SetValue(FirstRunValueName, startDate.ToString("yyyy-MM-dd"), RegistryValueKind.String);
                    return startDate;
                }
                else
                {
                    string storedDate = key.GetValue(FirstRunValueName).ToString();
                    if (DateTime.TryParse(storedDate, out DateTime firstRun))
                        return firstRun;
                    else
                        return officialStartDate;
                }
            }
        }
    }
}