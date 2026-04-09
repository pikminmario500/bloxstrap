using Bloxstrap.Models.SettingTasks.Base;

namespace Bloxstrap.Models.SettingTasks
{
    public class FontModPresetTask : StringBaseTask
    {
        private static string? ResolveInstalledFontPath(string familyName)
        {
            if (String.IsNullOrWhiteSpace(familyName))
                return null;

            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");
            if (key is null)
                return null;

            foreach (var valueName in key.GetValueNames())
            {
                var displayName = valueName;
                var parenIdx = displayName.LastIndexOf('(');
                if (parenIdx > 0)
                    displayName = displayName[..parenIdx].Trim();

                if (!String.Equals(displayName, familyName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var fileName = key.GetValue(valueName) as string;
                if (String.IsNullOrEmpty(fileName))
                    continue;

                var fullPath = Path.IsPathRooted(fileName) ? fileName : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            return null;
        }

        public string? GetFileHash()
        {
            if (!File.Exists(Paths.CustomFont))
                return null;

            using var fileStream = File.OpenRead(Paths.CustomFont);
            return MD5Hash.Stringify(App.MD5Provider.ComputeHash(fileStream));
        }

        public FontModPresetTask() : base("ModPreset", "TextFont")
        {
            if (File.Exists(Paths.CustomFont))
                OriginalState = App.Settings.Prop.CustomFontName;
        }

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(NewState))
            {
                var sourcePath = ResolveInstalledFontPath(NewState);

                if (sourcePath != null && String.Compare(sourcePath, Paths.CustomFont, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Paths.CustomFont)!);
                    Filesystem.AssertReadOnly(Paths.CustomFont);
                    File.Copy(sourcePath, Paths.CustomFont, true);
                }
            }
            else if (File.Exists(Paths.CustomFont))
            {
                Filesystem.AssertReadOnly(Paths.CustomFont);
                File.Delete(Paths.CustomFont);
            }

            OriginalState = NewState;
        }
    }
}
