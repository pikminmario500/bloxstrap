﻿using Bloxstrap.UI.ViewModels.Dialogs;

namespace Bloxstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for LanguageSelectorDialog.xaml
    /// </summary>
    public partial class LanguageSelectorDialog
    {
        public LanguageSelectorDialog()
        {
            var viewModel = new LanguageSelectorViewModel();

            DataContext = viewModel;
            InitializeComponent();

            viewModel.CloseRequestEvent += (_, _) => Close();
        }
    }
}
