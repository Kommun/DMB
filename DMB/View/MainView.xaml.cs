using System;
using System.Threading.Tasks;
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
        private bool _singleTap;
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
        /// Нажатие на ячейку календаря
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void calendarCell_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _singleTap = true;
            await Task.Delay(200);
            if (_singleTap)
                _vm.ShowCalendarMessage();
        }

        /// <summary>
        /// Двойное нажатие на дату в календаре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendarCell_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _singleTap = false;
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
