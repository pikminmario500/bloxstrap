using System.Windows;
using Microsoft.Win32;

namespace Bloxstrap
{
    internal class InstallChecker : IDisposable
    {
        private RegistryKey? _registryKey;
        private string? _installLocation;

        internal InstallChecker()
        {
            _registryKey = Registry.CurrentUser.OpenSubKey($"Software\\{App.ProjectName}", true);

            if (_registryKey is not null)
                _installLocation = (string?)_registryKey.GetValue("InstallLocation");
        }

        internal void Check()
        {
            const string LOG_IDENT = "InstallChecker::Check";

            if (_registryKey is null || _installLocation is null)
            {
                if (!File.Exists("Settings.json") || !File.Exists("State.json"))
                {
                    FirstTimeRun();
                    return;
                }

                App.Logger.WriteLine(LOG_IDENT, "Installation registry key is likely malformed");

                _installLocation = Path.GetDirectoryName(Paths.Process)!;

                var result = Frontend.ShowMessageBox(
                    string.Format(Resources.Strings.InstallChecker_NotInstalledProperly, _installLocation), 
                    MessageBoxImage.Warning, 
                    MessageBoxButton.YesNo
                );

                if (result != MessageBoxResult.Yes)
                {
                    FirstTimeRun();
                    return;
                }

                App.Logger.WriteLine(LOG_IDENT, $"Setting install location as '{_installLocation}'");

                if (_registryKey is null)
                    _registryKey = Registry.CurrentUser.CreateSubKey($"Software\\{App.ProjectName}");

                _registryKey.SetValue("InstallLocation", _installLocation);
            }

            // check if drive that bloxstrap was installed to was removed from system, or had its drive letter changed

            if (!Directory.Exists(_installLocation))
            {
                App.Logger.WriteLine(LOG_IDENT, "Could not find install location. Checking if drive has changed...");

                bool driveExists = false;
                string driveName = _installLocation[..3];
                string? newDriveName = null;

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.Name == driveName)
                        driveExists = true;
                    else if (Directory.Exists(_installLocation.Replace(driveName, drive.Name)))
                        newDriveName = drive.Name;
                }

                if (newDriveName is not null)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Drive has changed from {driveName} to {newDriveName}");

                    Frontend.ShowMessageBox(
                        string.Format(Resources.Strings.InstallChecker_DriveLetterChangeDetected, driveName, newDriveName),
                        MessageBoxImage.Warning,
                        MessageBoxButton.OK
                    );

                    _installLocation = _installLocation.Replace(driveName, newDriveName);
                    _registryKey.SetValue("InstallLocation", _installLocation);
                }
                else if (!driveExists)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Drive {driveName} does not exist anymore, and has likely been removed");

                    var result = Frontend.ShowMessageBox(
                        string.Format(Resources.Strings.InstallChecker_InstallDriveMissing, driveName),
                        MessageBoxImage.Warning,
                        MessageBoxButton.OKCancel
                    );

                    if (result != MessageBoxResult.OK)
                        App.Terminate();

                    FirstTimeRun();
                    return;
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT, "Drive has not changed, folder was likely moved or deleted");
                }
            }

            App.BaseDirectory = _installLocation;
            App.IsFirstRun = false;
        }

        public void Dispose()
        {
            _registryKey?.Dispose();
            GC.SuppressFinalize(this);
        }

        private static void FirstTimeRun()
        {
            const string LOG_IDENT = "InstallChecker::FirstTimeRun";

            App.Logger.WriteLine(LOG_IDENT, "Running first-time install");

            App.BaseDirectory = Path.Combine(Paths.LocalAppData, App.ProjectName);
            App.Logger.Initialize(true);

            if (App.LaunchSettings.IsQuiet)
                return;

            App.IsSetupComplete = false;

            App.FastFlags.Load();
            Frontend.ShowLanguageSelection();
            Frontend.ShowMenu();

            // exit if we don't click the install button on installation
            if (App.IsSetupComplete)
                return;
             
            App.Logger.WriteLine(LOG_IDENT, "Installation cancelled!");
            App.Terminate(ErrorCode.ERROR_CANCELLED);
        }
    }
}
