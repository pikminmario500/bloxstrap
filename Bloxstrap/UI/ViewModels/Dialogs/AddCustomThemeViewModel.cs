using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Bloxstrap.UI.ViewModels.Dialogs
{
    internal class AddCustomThemeViewModel : NotifyPropertyChangedViewModel
    {
        public static CustomThemeTemplate[] Templates => Enum.GetValues<CustomThemeTemplate>();

        public CustomThemeTemplate Template { get; set; } = CustomThemeTemplate.Simple;

        public BackgroundType WindowBackdropType { get; } = App.Settings.Prop.UseAero ? BackgroundType.Aero : BackgroundType.Mica;

        public SolidColorBrush BackgroundColourBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public AddCustomThemeViewModel()
        {
            if (App.Settings.Prop.UseAero)
            {
                BackgroundColourBrush = App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Light ?
                    new SolidColorBrush(Color.FromArgb(128, 225, 225, 225)) :
                    new SolidColorBrush(Color.FromArgb(128, 30, 30, 30));
            }
        }

        public string Name { get; set; } = "";

        private string _filePath = "";
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(FilePathVisibility));
                }
            }
        }
        public Visibility FilePathVisibility => string.IsNullOrEmpty(FilePath) ? Visibility.Collapsed : Visibility.Visible;

        public int SelectedTab { get; set; } = 0;

        private string _nameError = "";
        public string NameError
        {
            get => _nameError;
            set
            {
                if (_nameError != value)
                {
                    _nameError = value;
                    OnPropertyChanged(nameof(NameError));
                    OnPropertyChanged(nameof(NameErrorVisibility));
                }
            }
        }
        public Visibility NameErrorVisibility => string.IsNullOrEmpty(NameError) ? Visibility.Collapsed : Visibility.Visible;

        private string _fileError = "";
        public string FileError
        {
            get => _fileError;
            set
            {
                if (_fileError != value)
                {
                    _fileError = value;
                    OnPropertyChanged(nameof(FileError));
                    OnPropertyChanged(nameof(FileErrorVisibility));
                }
            }
        }
        public Visibility FileErrorVisibility => string.IsNullOrEmpty(FileError) ? Visibility.Collapsed : Visibility.Visible;
    }
}
