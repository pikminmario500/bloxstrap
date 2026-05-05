using Bloxstrap.Models.APIs.GitHub;
using Migrator.Utility;

using System.Net;

internal class Program
{
    private const string TargetNETVersion = "10.";

    public static readonly HttpClient HttpClient = new(
        new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
    );

    static async Task Main()
    {
        HttpClient.Timeout = TimeSpan.FromSeconds(30);
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "BloxstrapStub");

        if (await EnsureNETVersion())
            await UpdateBloxstrap();
    }

    static async Task<bool> EnsureNETVersion()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key is not null)
                foreach (var subKeyName in key.GetSubKeyNames())
                    using (var subKey = key.OpenSubKey(subKeyName))
                        if ((subKey?.GetValue("DisplayName") as string)?.StartsWith($"Microsoft Windows Desktop Runtime - {TargetNETVersion}", StringComparison.Ordinal) == true)
                            return true;
        }
        catch { } // we only need to show the dialog if we cant install the runtime

        try
        {
            string url = $"https://aka.ms/dotnet/{TargetNETVersion}0/windowsdesktop-runtime-win-x64.exe";
            string tempPath = Path.Combine(Path.GetTempPath(), "windowsdesktop-runtime-win-x64.exe");

            using (var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
                await response.Content.CopyToAsync(fs);
            }

            using (var process = Process.Start(new ProcessStartInfo { FileName = tempPath, UseShellExecute = true }))
            process?.WaitForExit();

            File.Delete(tempPath);
        }
        catch (Exception ex)
        {
            ErrorDialog.Show(string.Format(Strings.Dialog_Connectivity_UnableToConnect, "Microsoft"), ex);
            return false;
        }

        return true;
    }

    static async Task UpdateBloxstrap()
    {
        var releaseInfo = await Http.GetJson<GithubRelease>("https://api.github.com/repos/bloxstraplabs/bloxstrap/releases/latest");

        try
        {
            var asset = releaseInfo?.Assets?[1];
            if (asset is null)
                throw new HttpRequestException("Couldn't fetch assets from GitHub.");

            var path = Path.Combine(AppContext.BaseDirectory, asset.Name);

            using (var response = await HttpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
                await response.Content.CopyToAsync(fs);
            }

            if (!File.Exists(path))
                throw new HttpRequestException("Couldn't download the latest release.");

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = AppContext.BaseDirectory,
                UseShellExecute = true
            })?.WaitForExit();

            File.Delete(path);
        }
        catch (Exception ex)
        {
            ErrorDialog.Show(string.Format(Strings.Bootstrapper_AutoUpdateFailed, releaseInfo.TagName), ex);
        }
    }
}
