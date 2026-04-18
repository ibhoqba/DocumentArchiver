using System.Threading.Tasks;

namespace DocumentArchiever.Services
{
    public interface IUpdateService
    {
        Task CheckForUpdatesAsync();
        Task<bool> DownloadUpdateAsync();
        Task<bool> InstallUpdateAsync();
    }
}