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
using Shared.Utils;
using DMB.ViewModel;

namespace DMB.View
{
    public sealed partial class MainView : Page
    {
        private MainViewModel _vm;

        public MainView()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm = new MainViewModel(calendar, screenShotContainer);
            DataContext = _vm;
            // Обновляем иконки на случай, если изменили срок службы существующего солдата
            await TileHelper.Update();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Убираем ссылку на файл, чтобы он стал доступен для записи
            if (_vm != null)
                _vm.BackgroundImage = null;
        }

        /// <summary>
        /// Двойное нажатие на дату в календаре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _vm.AddEvent();
        }

        /// <summary>
        /// Конец манипуляции с календарем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var velocity = e.Velocities;
            if (Math.Abs(velocity.Linear.X) > Math.Abs(velocity.Linear.Y))
                if (velocity.Linear.X < 0)
                    _vm.SelectedPage++;
                else
                    _vm.SelectedPage--;
        }

        /// <summary>
        /// Начало манипуляции с кадендарем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Complete();
        }

        /// <summary>
        /// Загрузка списка событий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvEvents_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_vm.SelectedEvent != null)
                {
                    lvEvents.UpdateLayout();
                    lvEvents.ScrollIntoView(_vm.NearestEvent, ScrollIntoViewAlignment.Leading);
                }
            }
            catch { }
        }
    }
}
