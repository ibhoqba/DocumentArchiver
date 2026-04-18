
using DocumentArchiever.Configuration;
using DocumentArchiever.Core.OCR;
using DocumentArchiever.Core.Scanning;
using DocumentArchiever.Data;
using DocumentArchiever.Services;
using DocumentArchiever.UI.Forms;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace DocumentArchiever
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // License check
            if (!LicenseValidator.IsValid())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Setup dependency injection
            var container = new ServiceContainer();

            // Register services
            var logger = new FileLogger();
            container.RegisterSingleton<ILogger>(logger);
            container.RegisterSingleton<IDatabaseService>(new DatabaseService(logger));
            container.RegisterSingleton<IOcrEngine>(new TesseractOcrEngine(logger));
            container.RegisterSingleton<IScanner>(new HybridScanner(logger));
            container.RegisterSingleton<IUpdateService>(new UpdateService(logger));

            // Initialize OCR
            TesseractOcrEngine.Initialize();

            // Run application
            var mainForm = new MainForm(
                container.Resolve<IScanner>(),
                container.Resolve<IOcrEngine>(),
                container.Resolve<IDatabaseService>(),
                container.Resolve<IUpdateService>(),
                container.Resolve<ILogger>()
            );
            Application.Run(mainForm);
        }
    }
}