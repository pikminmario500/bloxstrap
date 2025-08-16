using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Bloxstrap.Integrations;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Appearance;

namespace Bloxstrap.UI.ViewModels.ContextMenu
{
    internal class ServerInformationViewModel : NotifyPropertyChangedViewModel
    {
        private readonly ActivityWatcher _activityWatcher;

        public string InstanceId => _activityWatcher.Data.JobId;

        public string ServerType => _activityWatcher.Data.ServerType.ToTranslatedString();

        public string ServerLocation { get; private set; } = Strings.Common_Loading;

        public Visibility ServerLocationVisibility => App.Settings.Prop.ShowServerDetails ? Visibility.Visible : Visibility.Collapsed;

        public ICommand CopyInstanceIdCommand => new RelayCommand(CopyInstanceId);

        public BackgroundType WindowBackdropType { get; } = App.Settings.Prop.UseAero ? BackgroundType.Aero : BackgroundType.Mica;

        public SolidColorBrush BackgroundColourBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public ServerInformationViewModel(Watcher watcher)
        {
            if (App.Settings.Prop.UseAero)
            {
                BackgroundColourBrush = App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Light ?
                    new SolidColorBrush(Color.FromArgb(128, 225, 225, 225)) :
                    new SolidColorBrush(Color.FromArgb(128, 30, 30, 30));
            }

            _activityWatcher = watcher.ActivityWatcher!;

            if (ServerLocationVisibility == Visibility.Visible)
                QueryServerLocation();
        }

        public async void QueryServerLocation()
        {
            string? location = await _activityWatcher.Data.QueryServerLocation();

            if (String.IsNullOrEmpty(location))
                ServerLocation = Strings.Common_NotAvailable;
            else
                ServerLocation = location;

            OnPropertyChanged(nameof(ServerLocation));
        }

        private void CopyInstanceId() => Clipboard.SetDataObject(InstanceId);
    }
}
