using System.Windows;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class BootstrapperViewModel : NotifyPropertyChangedViewModel
    {
        private string _oldPlayerVersionGuid = "";
        private string _oldStudioVersionGuid = "";

        public bool ConfirmLaunches
        {
            get => App.Settings.Prop.ConfirmLaunches;
            set => App.Settings.Prop.ConfirmLaunches = value;
        }

        public bool ForceRobloxLanguage
        {
            get => App.Settings.Prop.ForceRobloxLanguage;
            set => App.Settings.Prop.ForceRobloxLanguage = value;
        }

        public bool WPFSoftwareRenderEnabled
        {
            get => App.Settings.Prop.WPFSoftwareRender;
            set => App.Settings.Prop.WPFSoftwareRender = value;
        }

        public bool ForceRobloxReinstallation
        {
            // wouldnt it be better to check old version guids?
            // what about fresh installs?
            get => string.IsNullOrEmpty(App.State.Prop.Player.VersionGuid) && string.IsNullOrEmpty(App.State.Prop.Studio.VersionGuid);
            set
            {
                if (value)
                {
                    _oldPlayerVersionGuid = App.State.Prop.Player.VersionGuid;
                    _oldStudioVersionGuid = App.State.Prop.Studio.VersionGuid;
                    App.State.Prop.Player.VersionGuid = "";
                    App.State.Prop.Studio.VersionGuid = "";
                }
                else
                {
                    App.State.Prop.Player.VersionGuid = _oldPlayerVersionGuid;
                    App.State.Prop.Studio.VersionGuid = _oldStudioVersionGuid;
                }
            }
        }

        // divider

        public static Visibility ShowDebugStuff => App.Settings.Prop.ShowDebugStuff ? Visibility.Visible : Visibility.Collapsed;

        public string SelectedChannel
        {
            get => App.Settings.Prop.CustomChannel;
            set => App.Settings.Prop.CustomChannel = value;
        }

        public bool ForceLink
        {
            get => App.Settings.Prop.ForceNonCDNLink;
            set => App.Settings.Prop.ForceNonCDNLink = value;
        }
    }
}
