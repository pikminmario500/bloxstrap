using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Bloxstrap.UI.ViewModels.Dialogs
{
    public class MainViewModel : NotifyPropertyChangedViewModel
    {
        public BackgroundType WindowBackdropType { get; } = App.Settings.Prop.UseAero ? BackgroundType.Aero : BackgroundType.Mica;

        public SolidColorBrush BackgroundColourBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public MainViewModel()
        {
            if (App.Settings.Prop.UseAero)
            {
                BackgroundColourBrush = App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Light ?
                    new SolidColorBrush(Color.FromArgb(128, 225, 225, 225)) :
                    new SolidColorBrush(Color.FromArgb(128, 30, 30, 30));
            }
        }
    }
}
