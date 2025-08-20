using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SharpSevenZip;
using Microsoft.Win32;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class BloxstrapViewModel : NotifyPropertyChangedViewModel
    {
        public WebEnvironment[] WebEnvironments => Enum.GetValues<WebEnvironment>();

        public bool UpdateCheckingEnabled
        {
            get => App.Settings.Prop.CheckForUpdates;
            set => App.Settings.Prop.CheckForUpdates = value;
        }

        public bool AnalyticsEnabled
        {
            get => App.Settings.Prop.EnableAnalytics;
            set => App.Settings.Prop.EnableAnalytics = value;
        }

        public WebEnvironment WebEnvironment
        {
            get => App.Settings.Prop.WebEnvironment;
            set => App.Settings.Prop.WebEnvironment = value;
        }

        public Visibility WebEnvironmentVisibility => App.Settings.Prop.DeveloperMode ? Visibility.Visible : Visibility.Collapsed;

        public bool ShouldExportConfig { get; set; } = true;

        public bool ShouldExportLogs { get; set; } = true;

        public ICommand ExportDataCommand => new RelayCommand(ExportData);

        private void ExportData()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            var dialog = new SaveFileDialog 
            { 
                FileName = $"Bloxstrap-export-{timestamp}.zip",
                Filter = $"{Strings.FileTypes_ZipArchive}|*.zip" 
            };

            if (dialog.ShowDialog() != true)
                return;

            var zip = new SharpSevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip
            };

            if (ShouldExportConfig)
            {
                var files = new List<string>()
                {
                    App.Settings.FileLocation,
                    App.State.FileLocation,
                    App.FastFlags.FileLocation
                };

                Directory.CreateDirectory($"{Paths.Temp}/Export/Config");
                foreach (string file in files)
                    File.Copy(file, $"{Paths.Temp}/Export/Config/{Path.GetFileName(file)}");
            }

            if (ShouldExportLogs && Directory.Exists(Paths.Logs))
            {
                var files = Directory.GetFiles(Paths.Logs)
                    .Where(x => !x.Equals(App.Logger.FileLocation, StringComparison.OrdinalIgnoreCase));

                Directory.CreateDirectory($"{Paths.Temp}/Export/Logs");
                foreach (string file in files)
                    File.Copy(file, $"{Paths.Temp}/Export/Logs/{Path.GetFileName(file)}");
            }

            using var outputStream = File.OpenWrite(dialog.FileName);
            zip.CompressDirectory($"{Paths.Temp}/Export", outputStream);
            Directory.Delete($"{Paths.Temp}/Export", true);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }
    }
}
