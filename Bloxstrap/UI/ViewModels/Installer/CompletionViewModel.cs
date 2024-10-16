using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Bloxstrap.UI.ViewModels.Installer
{
    public class CompletionViewModel
    {
        public ICommand LaunchSettingsCommand => new RelayCommand(LaunchSettings);

        public ICommand LaunchRobloxCommand => new RelayCommand(LaunchRoblox);

        public event EventHandler<NextAction>? CloseWindowRequest;

        private void LaunchSettings() => CloseWindowRequest?.Invoke(this, NextAction.LaunchSettings);

        private void LaunchRoblox() => CloseWindowRequest?.Invoke(this, NextAction.LaunchRoblox);
    }
}
