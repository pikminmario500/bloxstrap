using Bloxstrap.UI.Elements.Base;
using Bloxstrap.UI.ViewModels.Dialogs;
using Microsoft.Win32;
using System.IO.Compression;
using System.Windows;

using SharpCompress.Readers;
using SharpCompress.Archives;

namespace Bloxstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for AddCustomThemeDialog.xaml
    /// </summary>
    public partial class AddCustomThemeDialog : WpfUiWindow
    {
        private const int CreateNewTabId = 0;
        private const int ImportTabId = 1;

        private readonly AddCustomThemeViewModel _viewModel;

        private readonly string[] supportedArchives = [
            ".zip",
            ".rar",
            ".7z",
            ".tar",
            ".tar.gz",
            ".tar.bz2",
            ".tar.lz",
            ".tar.xz",
            ".gz",
            ".arc"
        ];

        public bool Created { get; private set; } = false;
        public string ThemeName { get; private set; } = "";
        public bool OpenEditor { get; private set; } = false;

        public AddCustomThemeDialog()
        {
            _viewModel = new AddCustomThemeViewModel();
            _viewModel.Name = GenerateRandomName();

            DataContext = _viewModel;

            if (App.Settings.Prop.UseAero)
                AllowsTransparency = true;

            InitializeComponent();
        }

        private static string GetThemePath(string name)
        {
            return Path.Combine(Paths.CustomThemes, name, "Theme.xml");
        }

        private static string GenerateRandomName()
        {
            int count = Directory.GetDirectories(Paths.CustomThemes).Count();

            int i = count + 1;
            string name = string.Format(Strings.CustomTheme_DefaultName, i);

            // TODO: this sucks
            if (File.Exists(GetThemePath(name)))
                name = string.Format(Strings.CustomTheme_DefaultName, $"{i}-{Random.Shared.Next(1, 100000)}"); // easy

            return name;
        }

        private static string GetUniqueName(string name)
        {
            const int maxTries = 100;

            if (!File.Exists(GetThemePath(name)))
                return name;

            for (int i = 1; i <= maxTries; i++)
            {
                string newName = $"{name}_{i}";
                if (!File.Exists(GetThemePath(newName)))
                    return newName;
            }

            // last resort
            return $"{name}_{Random.Shared.Next(maxTries+1, 1_000_000)}";
        }

        private static void CreateCustomTheme(string name, CustomThemeTemplate template)
        {
            string dir = Path.Combine(Paths.CustomThemes, name);

            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);

            string themeFilePath = Path.Combine(dir, "Theme.xml");

            string templateContent = template.GetFileContents();

            File.WriteAllText(themeFilePath, templateContent);
        }

        private bool ValidateCreateNew()
        {
            const string LOG_IDENT = "AddCustomThemeDialog::ValidateCreateNew";

            if (string.IsNullOrEmpty(_viewModel.Name))
            {
                _viewModel.NameError = Strings.CustomTheme_Add_Errors_NameEmpty;
                return false;
            }

            var validationResult = PathValidator.IsFileNameValid(_viewModel.Name);

            if (validationResult != PathValidator.ValidationResult.Ok)
            {
                switch (validationResult)
                {
                    case PathValidator.ValidationResult.IllegalCharacter:
                        _viewModel.NameError = Strings.CustomTheme_Add_Errors_NameIllegalCharacters;
                        break;
                    case PathValidator.ValidationResult.ReservedFileName:
                        _viewModel.NameError = Strings.CustomTheme_Add_Errors_NameReserved;
                        break;
                    default:
                        App.Logger.WriteLine(LOG_IDENT, $"Got unhandled PathValidator::ValidationResult {validationResult}");
                        Debug.Assert(false);

                        _viewModel.NameError = Strings.CustomTheme_Add_Errors_Unknown;
                        break;
                }

                return false;
            }

            // better to check for the file instead of the directory so broken themes can be overwritten
            string path = Path.Combine(Paths.CustomThemes, _viewModel.Name, "Theme.xml");
            if (File.Exists(path))
            {
                _viewModel.NameError = Strings.CustomTheme_Add_Errors_NameTaken;
                return false;
            }    

            return true;
        }

        private bool ValidateImport()
        {
            const string LOG_IDENT = "AddCustomThemeDialog::ValidateImport";

            if (!supportedArchives.Any(_viewModel.FilePath.EndsWith))
            {
                _viewModel.FileError = Strings.CustomTheme_Add_Errors_FileNotZip;
                return false;
            }

            try
            {
                using var zipFile = ReaderFactory.Open(File.OpenRead(_viewModel.FilePath));

                bool foundThemeFile = false;

                while (zipFile.MoveToNextEntry())
                {
                    if (zipFile.Entry.Key == "Theme.xml")
                    {
                        foundThemeFile = true;
                        break;
                    }
                }

                if (!foundThemeFile)
                {
                    _viewModel.FileError = Strings.CustomTheme_Add_Errors_ZipMissingThemeFile;
                    return false;
                }

                return true;
            }
            catch (InvalidDataException ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Got invalid data");
                App.Logger.WriteException(LOG_IDENT, ex);

                _viewModel.FileError = Strings.CustomTheme_Add_Errors_ZipInvalidData;
                return false;
            }
        }

        private void CreateNew()
        {
            if (!ValidateCreateNew())
                return;

            CreateCustomTheme(_viewModel.Name, _viewModel.Template);

            Created = true;
            ThemeName = _viewModel.Name;
            OpenEditor = true;

            Close();
        }

        private void Import()
        {
            if (!ValidateImport())
                return;

            string fileName = Path.GetFileNameWithoutExtension(_viewModel.FilePath);
            string name = GetUniqueName(fileName);

            string directory = Path.Combine(Paths.CustomThemes, name);
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);

            ArchiveFactory.Open(_viewModel.FilePath).ExtractToDirectory(directory);

            Created = true;
            ThemeName = name;
            OpenEditor = false;

            Close();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTab == CreateNewTabId)
                CreateNew();
            else
                Import();
        }

        private void OnImportButtonClicked(object sender, RoutedEventArgs e)
        {
            string Filter = $"{Strings.FileTypes_ZipArchive}|";

            foreach (string type in supportedArchives)
            {
                Filter += $"*{type}; ";
            }

            Filter = Filter[..^2];

            var dialog = new OpenFileDialog
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.FilePath = dialog.FileName;
        }
    }
}
