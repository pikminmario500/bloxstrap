// To debug the automatic updater:
// - Uncomment the definition below
// - Publish the executable
// - Launch the executable (click no when it asks you to upgrade)
// - Launch Roblox (for testing web launches, run it from the command prompt)
// - To re-test the same executable, delete it from the installation folder

// #define DEBUG_UPDATER

#if DEBUG_UPDATER
#warning "Automatic updater debugging is enabled"
#endif

using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;

using Microsoft.Win32;

namespace Bloxstrap
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#if QA_BUILD
        public const string ProjectName = "Bloxstrap-QA";
#else
        public const string ProjectName = "Bloxstrap";
#endif
        public const string ProjectOwner = "Bloxstrap";
        public const string ProjectRepository = "bloxstraplabs/bloxstrap";
        public const string ProjectDownloadLink = "https://bloxstraplabs.com";
        public const string ProjectHelpLink = "https://bloxstraplabs.com/wiki/help/";
        public const string ProjectSupportLink = "https://github.com/bloxstraplabs/bloxstrap/issues/new";

        public const string RobloxPlayerAppName = "RobloxPlayerBeta";
        public const string RobloxStudioAppName = "RobloxStudioBeta";

        // simple shorthand for extremely frequently used and long string - this goes under HKCU
        public const string UninstallKey = $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{ProjectName}";

        public static LaunchSettings LaunchSettings { get; private set; } = null!;

        public static BuildMetadataAttribute BuildMetadata = Assembly.GetExecutingAssembly().GetCustomAttribute<BuildMetadataAttribute>()!;

        public static string Version = Assembly.GetExecutingAssembly().GetName().Version!.ToString()[..^2];

        public static Bootstrapper? Bootstrapper { get; set; } = null!;

        public static bool IsActionBuild => !String.IsNullOrEmpty(BuildMetadata.CommitRef);

        public static bool IsProductionBuild => IsActionBuild && BuildMetadata.CommitRef.StartsWith("tag", StringComparison.Ordinal);

        public static bool IsPlayerInstalled => App.PlayerState.IsSaved && !String.IsNullOrEmpty(App.PlayerState.Prop.VersionGuid);

        public static bool IsStudioInstalled => App.StudioState.IsSaved && !String.IsNullOrEmpty(App.StudioState.Prop.VersionGuid);

        public static readonly MD5 MD5Provider = MD5.Create();

        public static readonly Logger Logger = new();

        public static readonly Dictionary<string, BaseTask> PendingSettingTasks = new();

        public static readonly JsonManager<Settings> Settings = new();

        public static readonly JsonManager<State> State = new();

        public static readonly LazyJsonManager<DistributionState> PlayerState = new(nameof(PlayerState));

        public static readonly LazyJsonManager<DistributionState> StudioState = new(nameof(StudioState));

        public static readonly FastFlagManager FastFlags = new();

        public static readonly HttpClient HttpClient = new(
            new HttpClientLoggingHandler(
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
            )
        );

        private static bool _showingExceptionDialog = false;

        private static string? _webUrl = null;
        public static string WebUrl
        {
            get {
                if (_webUrl != null)
                    return _webUrl;

                string url = ConstructBloxstrapWebUrl();
                if (Settings.Loaded) // only cache if settings are done loading
                    _webUrl = url;
                return url;
            }
        }
        
        public static void Terminate(ErrorCode exitCode = ErrorCode.ERROR_SUCCESS)
        {
            int exitCodeNum = (int)exitCode;

            Logger.WriteLine("App::Terminate", $"Terminating with exit code {exitCodeNum} ({exitCode})");

            Environment.Exit(exitCodeNum);
        }

        public static void SoftTerminate(ErrorCode exitCode = ErrorCode.ERROR_SUCCESS)
        {
            int exitCodeNum = (int)exitCode;

            Logger.WriteLine("App::SoftTerminate", $"Terminating with exit code {exitCodeNum} ({exitCode})");

            Current.Dispatcher.Invoke(() => Current.Shutdown(exitCodeNum));
        }

        void GlobalExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            Logger.WriteLine("App::GlobalExceptionHandler", "An exception occurred");

            FinalizeExceptionHandling(e.Exception);
        }

        public static void FinalizeExceptionHandling(AggregateException ex)
        {
            foreach (var innerEx in ex.InnerExceptions)
                Logger.WriteException("App::FinalizeExceptionHandling", innerEx);

            FinalizeExceptionHandling(ex.GetBaseException(), false);
        }

        public static void FinalizeExceptionHandling(Exception ex, bool log = true)
        {
            if (log)
                Logger.WriteException("App::FinalizeExceptionHandling", ex);

            if (_showingExceptionDialog)
                return;

            _showingExceptionDialog = true;

            SendLog();

            if (Bootstrapper?.Dialog != null)
            {
                if (Bootstrapper.Dialog.TaskbarProgressValue == 0)
                    Bootstrapper.Dialog.TaskbarProgressValue = 1; // make sure it's visible

                Bootstrapper.Dialog.TaskbarProgressState = TaskbarItemProgressState.Error;
            }

            Frontend.ShowExceptionDialog(ex);

            Terminate(ErrorCode.ERROR_INSTALL_FAILURE);
        }

        public static string ConstructBloxstrapWebUrl()
        {
            // dont let user switch web environment if debug mode is not on
            if (Settings.Prop.WebEnvironment == WebEnvironment.Production || !Settings.Prop.DeveloperMode)
                return "services.bloxstraplabs.com";

            string? sub = Settings.Prop.WebEnvironment.GetDescription();
            return $"services-{sub}.bloxstraplabs.com";
        }

        public static bool CanSendLogs()
        {
            // non developer mode always uses production
            if (!Settings.Prop.DeveloperMode || Settings.Prop.WebEnvironment == WebEnvironment.Production)
                return IsProductionBuild;

            return true;
        }

        public static async Task<GithubRelease?> GetLatestRelease()
        {
            const string LOG_IDENT = "App::GetLatestRelease";

            try
            {
                var releaseInfo = await Http.GetJson<GithubRelease>($"https://api.github.com/repos/{ProjectRepository}/releases/latest");

                if (releaseInfo is null || releaseInfo.Assets is null)
                {
                    Logger.WriteLine(LOG_IDENT, "Encountered invalid data");
                    return null;
                }

                return releaseInfo;
            }
            catch (Exception ex)
            {
                Logger.WriteException(LOG_IDENT, ex);
            }

            return null;
        }

        public static async void SendStat(string key, string value)
        {
            if (!Settings.Prop.EnableAnalytics)
                return;

            try
            {
                await HttpClient.GetAsync($"https://{WebUrl}/metrics/post?key={key}&value={value}");
            }
            catch (Exception ex)
            {
                Logger.WriteException("App::SendStat", ex);
            }
        }

        public static async void SendLog()
        {
            if (!Settings.Prop.EnableAnalytics || !CanSendLogs())
                return;

            try
            {
                await HttpClient.PostAsync(
                    $"https://{WebUrl}/metrics/post-exception", 
                    new StringContent(Logger.AsDocument)
                );
            }
            catch (Exception ex)
            {
                Logger.WriteException("App::SendLog", ex);
            }
        }

        public static void AssertWindowsOSVersion()
        {
            const string LOG_IDENT = "App::AssertWindowsOSVersion";

            int major = Environment.OSVersion.Version.Major;
            if (major < 10) // Windows 10 and newer only
            {
                Logger.WriteLine(LOG_IDENT, $"Detected unsupported Windows version ({Environment.OSVersion.Version}).");

                if (!LaunchSettings.QuietFlag.Active)
                    Frontend.ShowMessageBox(Strings.App_OSDeprecation_Win7_81, MessageBoxImage.Error);

                Terminate(ErrorCode.ERROR_INVALID_FUNCTION);
            }
        }

        public static async Task<bool> CheckForUpdates(bool IsBootstrapper = false)
        {
            const string LOG_IDENT = "App::CheckForUpdates";
            
            // don't update if there's another instance running (likely running in the background)
            // i don't like this, but there isn't much better way of doing it /shrug
            if (Process.GetProcessesByName(ProjectName).Length > 1)
            {
                Logger.WriteLine(LOG_IDENT, $"More than one Bloxstrap instance running, aborting update check");
                return false;
            }

            Logger.WriteLine(LOG_IDENT, "Checking for updates...");

#if !DEBUG_UPDATER
            var releaseInfo = await GetLatestRelease();

            if (releaseInfo is null)
                return false;

            var versionComparison = Utilities.CompareVersions(Version, releaseInfo.TagName);

            // check if we aren't using a deployed build, so we can update to one if a new version comes out
            if (IsProductionBuild && versionComparison == VersionComparison.Equal || versionComparison == VersionComparison.GreaterThan)
            {
                Logger.WriteLine(LOG_IDENT, "No updates found");
                return false;
            }

            if (IsBootstrapper && Bootstrapper!.Dialog is not null)
                Bootstrapper.Dialog.CancelEnabled = false;

            string version = releaseInfo.TagName;
#else
            string version = Version;
#endif

            if (IsBootstrapper)
                Bootstrapper!.SetStatus(Strings.Bootstrapper_Status_UpgradingBloxstrap);

            try
            {
#if DEBUG_UPDATER
                string downloadLocation = Path.Combine(Paths.TempUpdates, "Bloxstrap.exe");

                Directory.CreateDirectory(Paths.TempUpdates);

                File.Copy(Paths.Process, downloadLocation, true);
#else
                var asset = releaseInfo.Assets![0];

                string downloadLocation = Path.Combine(Paths.TempUpdates, asset.Name);

                Directory.CreateDirectory(Paths.TempUpdates);

                Logger.WriteLine(LOG_IDENT, $"Downloading {releaseInfo.TagName}...");
                
                if (!File.Exists(downloadLocation))
                {
                    var response = await HttpClient.GetAsync(asset.BrowserDownloadUrl);

                    await using var fileStream = new FileStream(downloadLocation, FileMode.OpenOrCreate, FileAccess.Write);
                    await response.Content.CopyToAsync(fileStream);
                }
#endif

                Logger.WriteLine(LOG_IDENT, $"Starting {version}...");

                ProcessStartInfo startInfo = new()
                {
                    FileName = downloadLocation,
                };

                if (IsBootstrapper)
                    startInfo.ArgumentList.Add("-upgrade");

                foreach (string arg in LaunchSettings.Args)
                    startInfo.ArgumentList.Add(arg);

                if (IsBootstrapper)
                {
                    if (Bootstrapper!._launchMode == LaunchMode.Player && !startInfo.ArgumentList.Contains("-player"))
                        startInfo.ArgumentList.Add("-player");
                    else if (Bootstrapper._launchMode == LaunchMode.Studio && !startInfo.ArgumentList.Contains("-studio"))
                        startInfo.ArgumentList.Add("-studio");
                }

                Settings.Save();

                new InterProcessLock("AutoUpdater");
                
                Process.Start(startInfo);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LOG_IDENT, "An exception occurred when running the auto-updater");
                Logger.WriteException(LOG_IDENT, ex);

                Frontend.ShowMessageBox(
                    string.Format(Strings.Bootstrapper_AutoUpdateFailed, version),
                    MessageBoxImage.Information
                );

                Utilities.ShellExecute(ProjectDownloadLink);
            }

            return false;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            const string LOG_IDENT = "App::OnStartup";

            Locale.Initialize();

            base.OnStartup(e);

            Logger.WriteLine(LOG_IDENT, $"Starting {ProjectName} v{Version}");

            string userAgent = $"{ProjectName}/{Version}";

            if (IsActionBuild)
            {
                Logger.WriteLine(LOG_IDENT, $"Compiled {BuildMetadata.Timestamp.ToFriendlyString()} from commit {BuildMetadata.CommitHash} ({BuildMetadata.CommitRef})");

                if (IsProductionBuild)
                    userAgent += $" (Production)";
                else
                    userAgent += $" (Artifact {BuildMetadata.CommitHash}, {BuildMetadata.CommitRef})";
            }
            else
            {
                Logger.WriteLine(LOG_IDENT, $"Compiled {BuildMetadata.Timestamp.ToFriendlyString()} from {BuildMetadata.Machine}");

#if QA_BUILD
                userAgent += " (QA)";
#else
                userAgent += $" (Build {Convert.ToBase64String(Encoding.UTF8.GetBytes(BuildMetadata.Machine))})";
#endif
            }

            Logger.WriteLine(LOG_IDENT, $"OSVersion: {Environment.OSVersion}");

            Logger.WriteLine(LOG_IDENT, $"Loaded from {Paths.Process}");
            Logger.WriteLine(LOG_IDENT, $"Temp path is {Paths.Temp}");
            Logger.WriteLine(LOG_IDENT, $"WindowsStartMenu path is {Paths.WindowsStartMenu}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            HttpClient.Timeout = TimeSpan.FromSeconds(30);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

            LaunchSettings = new LaunchSettings(e.Args);

            // installation check begins here
            using var uninstallKey = Registry.CurrentUser.OpenSubKey(UninstallKey);
            string? installLocation = null;
            bool fixInstallLocation = false;
            
            if (uninstallKey?.GetValue("InstallLocation") is string value)
            {
                if (Directory.Exists(value))
                {
                    installLocation = value;
                }
                else
                {
                    // check if user profile folder has been renamed
                    var match = Regex.Match(value, @"^[a-zA-Z]:\\Users\\([^\\]+)", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string newLocation = value.Replace(match.Value, Paths.UserProfile, StringComparison.InvariantCultureIgnoreCase);

                        if (Directory.Exists(newLocation))
                        {
                            installLocation = newLocation;
                            fixInstallLocation = true;
                        }
                    }
                }
            }

            // silently change install location if we detect a portable run
            if (installLocation is null && Directory.GetParent(Paths.Process)?.FullName is string processDir)
            {
                var files = Directory.GetFiles(processDir).Select(x => Path.GetFileName(x)).ToArray();

                // check if settings.json and state.json are the only files in the folder
                if (files.Length <= 3 && files.Contains("Settings.json") && files.Contains("State.json"))
                {
                    installLocation = processDir;
                    fixInstallLocation = true;
                }
            }

            if (fixInstallLocation && installLocation is not null)
            {
                var installer = new Installer
                {
                    InstallLocation = installLocation,
                    IsImplicitInstall = true
                };

                if (installer.CheckInstallLocation())
                {
                    Logger.WriteLine(LOG_IDENT, $"Changing install location to '{installLocation}'");
                    installer.DoInstall();
                }
                else
                {
                    // force reinstall
                    installLocation = null;
                }
            }

            if (installLocation is null)
            {
                Logger.Initialize(true);
                Logger.WriteLine(LOG_IDENT, "Not installed, launching the installer");
                AssertWindowsOSVersion(); // prevent new installs from unsupported operating systems
                LaunchHandler.LaunchInstaller();
            }
            else
            {
                Paths.Initialize(installLocation);

                Logger.WriteLine(LOG_IDENT, "Entering main logic");

                // ensure executable is in the install directory
                if (Paths.Process != Paths.Application && !File.Exists(Paths.Application))
                {
                    Logger.WriteLine(LOG_IDENT, "Copying to install directory");
                    File.Copy(Paths.Process, Paths.Application);
                }

                Logger.Initialize(LaunchSettings.UninstallFlag.Active);

                if (!Logger.Initialized && !Logger.NoWriteMode)
                {
                    Logger.WriteLine(LOG_IDENT, "Possible duplicate launch detected, terminating.");
                    Terminate();
                }

                Settings.Load();
                State.Load();
                FastFlags.Load();

                if (!Locale.SupportedLocales.ContainsKey(Settings.Prop.Locale))
                {
                    Settings.Prop.Locale = "nil";
                    Settings.Save();
                }

                Logger.WriteLine(LOG_IDENT, $"Developer mode: {Settings.Prop.DeveloperMode}");
                Logger.WriteLine(LOG_IDENT, $"Web environment: {Settings.Prop.WebEnvironment}");

                Locale.Set(Settings.Prop.Locale);

                if (!LaunchSettings.BypassUpdateCheck)
                    Installer.HandleUpgrade();

                LaunchHandler.ProcessLaunchArgs();
            }

            // you must *explicitly* call terminate when everything is done, it won't be called implicitly
            Logger.WriteLine(LOG_IDENT, "Startup finished");
        }
    }
}
