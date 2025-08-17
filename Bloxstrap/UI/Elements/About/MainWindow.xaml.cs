using System.Windows.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

using Bloxstrap.UI.ViewModels.About;

namespace Bloxstrap.UI.Elements.About
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public MainWindow()
        {
            var viewModel = new MainWindowViewModel();
            
            DataContext = viewModel;

            if (App.Settings.Prop.UseAero)
                AllowsTransparency = true;

            InitializeComponent();

            App.Logger.WriteLine("MainWindow", "Initializing about window");

            if (Locale.CurrentCulture.Name.StartsWith("tr"))
                TranslatorsText.FontSize = 9;
        }

        #region INavigationWindow methods

        public Frame GetFrame() => RootFrame;

        public INavigation GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.PageService = pageService;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods
    }
}
