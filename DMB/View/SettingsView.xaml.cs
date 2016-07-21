using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace DMB.View
{
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();
            DataContext = new ViewModel.SettingsViewModel();
        }

        private void cmbTheme_DropDownClosed(object sender, object e)
        {
            // Обнуляем выбранный элемент и затем возвращаем его, чтобы сработал обработчик
            var selectedItem = cmbTheme.SelectedItem;
            cmbTheme.SelectedItem = null;
            cmbTheme.SelectedItem = selectedItem;
        }

        private async void Lock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await new MessageDialog("Данные настройки доступны только в полной версии").ShowAsync();
        }
    }
}
