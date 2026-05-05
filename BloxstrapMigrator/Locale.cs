using System.Globalization;
using System.Linq;
using System.Threading;

namespace Migrator
{
    internal static class Locale
    {
        public static bool RightToLeft { get; private set; } = false;

        private static readonly string[] _rtlLocales = {"ar", "he", "fa"};

        public static void Initialize()
        {
            string identifier = "nil";

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Bloxstrap");
                var installLocation = key?.GetValue("InstallLocation") as string;

                if (installLocation is not null)
                {
                    string settingsPath = Path.Combine(installLocation, "Settings.json");

                    if (File.Exists(settingsPath))
                    {
                        var json = JsonDocument.Parse(File.ReadAllText(settingsPath));
                        if (json.RootElement.TryGetProperty("Locale", out var locale))
                            identifier = locale.GetString() ?? "nil";
                    }
                }
            }
            catch { }

            Set(identifier);
        }

        public static void Set(string identifier)
        {
            var culture = identifier == "nil"
                ? CultureInfo.CurrentUICulture
                : new CultureInfo(identifier);

            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            RightToLeft = _rtlLocales.Any(l => culture.Name.StartsWith(l));
        }
    }
}
