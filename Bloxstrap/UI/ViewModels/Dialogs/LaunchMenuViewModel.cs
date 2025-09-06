using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Appearance;

using Bloxstrap.UI.Elements.About;

namespace Bloxstrap.UI.ViewModels.Installer
{
    public class LaunchMenuViewModel
    {
        public BackgroundType WindowBackdropType { get; } = App.Settings.Prop.UseAero ? BackgroundType.Aero : BackgroundType.Mica;

        public SolidColorBrush BackgroundColourBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public LaunchMenuViewModel()
        {
            if (App.Settings.Prop.UseAero)
            {
                BackgroundColourBrush = App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Light ?
                    new SolidColorBrush(Color.FromArgb(128, 225, 225, 225)) :
                    new SolidColorBrush(Color.FromArgb(128, 30, 30, 30));
            }
        }

        public string Version => string.Format(Strings.Menu_About_Version, App.ShortCommitHash);

        public Visibility RobloxStudioOptionVisibility => App.IsStudioVisible ? Visibility.Visible : Visibility.Collapsed;

        public ICommand LaunchSettingsCommand => new RelayCommand(LaunchSettings);

        public ICommand LaunchRobloxCommand => new RelayCommand(LaunchRoblox);

        public ICommand LaunchRobloxStudioCommand => new RelayCommand(LaunchRobloxStudio);

        public ICommand LaunchAboutCommand => new RelayCommand(LaunchAbout);

        public event EventHandler<NextAction>? CloseWindowRequest;

        private void LaunchSettings() => CloseWindowRequest?.Invoke(this, NextAction.LaunchSettings);

        private void LaunchRoblox() => CloseWindowRequest?.Invoke(this, NextAction.LaunchRoblox);

        private void LaunchRobloxStudio() => CloseWindowRequest?.Invoke(this, NextAction.LaunchRobloxStudio);

        private void LaunchAbout() => new MainWindow().ShowDialog();
    }
}
