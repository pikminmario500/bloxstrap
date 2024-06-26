﻿using System.Windows;
using System.Windows.Interop;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace Bloxstrap.UI.Elements.Base
{
    public class WpfUiWindow : UiWindow
    {
        private readonly IThemeService _themeService = new ThemeService();

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (App.Settings.Prop.SoftwareRenderingEnabled)
            {
                var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

                if (hwndSource != null)
                    hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            }

            base.OnSourceInitialized(e);
        }

        public void ApplyTheme()
        {
            _themeService.SetTheme(App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Dark ? ThemeType.Dark : ThemeType.Light);
            _themeService.SetSystemAccent();
        }
    }
}
