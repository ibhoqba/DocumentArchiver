using System.Threading.Tasks;

namespace DocumentArchiever.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ILogger _logger;
        private readonly AutoUpdater _autoUpdater;

        public UpdateService(ILogger logger)
        {
            _logger = logger;
            _autoUpdater = new AutoUpdater(logger);
        }

        public async Task CheckForUpdatesAsync()
        {
            await _autoUpdater.CheckForUpdatesAsync();
        }

        public async Task<bool> DownloadUpdateAsync()
        {
            // Implementation
            return await Task.FromResult(true);
        }

        public async Task<bool> InstallUpdateAsync()
        {
            // Implementation
            return await Task.FromResult(true);
        }
    }
}