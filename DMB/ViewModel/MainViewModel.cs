using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using Syncfusion.UI.Xaml.Controls.Input;
using Shared.Utils;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime;
using System.IO;

namespace DMB.ViewModel
{
    public class MainViewModel : PropertyChangedBase
    {


        #region Fields

        private UIElement _screenShotContainer;
        private SfCalendar _calendar;
        private DispatcherTimer _timer = new DispatcherTimer();
        private int _selectedPage;
        private voin _selectedFriend;
        private importantDate _selectedEvent;
        private string _backgroundImage;
        private Popup _currentPopup;

        #endregion

        #region Commands

        public RelayCommand ChangeMainSoldierCommand { get; set; }
        public RelayCommand PinCommand { get; set; }
        public RelayCommand DocumentsCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand ShareCommand { get; set; }

        public RelayCommand AddFriendCommand { get; set; }
        public RelayCommand ChangeFriendCommand { get; set; }
        public RelayCommand DeleteFriendCommand { get; set; }
        public RelayCommand MakeMainCommand { get; set; }

        public RelayCommand AddEventCommand { get; set; }

        public RelayCommand DeleteEventCommand { get; set; }

        #endregion

        #region Properties

        public AppSettings Settings { get; set; } = new AppSettings();

        /// <summary>
        /// Коллекция событий
        /// </summary>
        public ObservableCollection<importantDate> Events { get; set; } = new ObservableCollection<importantDate>();

        /// <summary>
        /// Коллекция друзей
        /// </summary>
        public ObservableCollection<voin> Friends { get; set; }

        /// <summary>
        /// Выбранное событие
        /// </summary>
        public importantDate SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                OnPropertyChanged("SelectedEvent");
            }
        }

        /// <summary>
        /// Ближайшее событие
        /// </summary>
        public importantDate NearestEvent { get; set; }

        /// <summary>
        /// Выбранный солдат
        /// </summary>
        public voin SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged("SelectedFriend");
            }
        }

        /// <summary>
        /// Выбранная в календаре дата
        /// </summary>
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Фоновое изображение
        /// </summary>
        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    OnPropertyChanged("BackgroundImage");
                }
            }
        }

        /// <summary>
        /// Главный солдат
        /// </summary>
        public voin MainSoldier { get; set; }
        public Visibility TimerVisibility { get; set; }

        /// <summary>
        /// Выбранная страница
        /// </summary>
        public int SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                OnPropertyChanged("SelectedPage");
            }
        }

        /// <summary>
        /// Отслужено
        /// </summary>
        public string DmbTime { get; set; }

        /// <summary>
        /// Прошло дней
        /// </summary>
        public string DaysLast { get; set; }

        /// <summary>
        /// Прошло
        /// </summary>
        public string TimeLast { get; set; }

        /// <summary>
        /// Осталось дней
        /// </summary>
        public string DaysRemain { get; set; }

        /// <summary>
        /// Осталось
        /// </summary>
        public string TimeRemain { get; set; }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainViewModel(SfCalendar calendar, UIElement screenShotContainer)
        {
            ChangeMainSoldierCommand = new RelayCommand(ChangeMainSoldier);
            PinCommand = new RelayCommand(async (o) => await TileHelper.Pin(MainSoldier));
            SettingsCommand = new RelayCommand((o) => App.NavigationService.Navigate(typeof(View.SettingsView)));
            DocumentsCommand = new RelayCommand((o) => App.NavigationService.Navigate(typeof(View.DocumentsListView)));
            ShareCommand = new RelayCommand(Share);

            AddFriendCommand = new RelayCommand(AddFriend);
            ChangeFriendCommand = new RelayCommand(ChangeFriend);
            DeleteFriendCommand = new RelayCommand(DeleteFriend);
            MakeMainCommand = new RelayCommand(MakeMain);

            AddEventCommand = new RelayCommand(AddEvent);
            DeleteEventCommand = new RelayCommand(DeleteEvent);

            _calendar = calendar;
            _screenShotContainer = screenShotContainer;

            try
            {
                MainSoldier = DataBaseHelper.Connection.Query<voin>(string.Format("Select * From voin where Id={0}", Settings.mainSoldierId)).FirstOrDefault();
                if (MainSoldier == null)
                    return;

                timer_Tick(null, null);
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += timer_Tick;
                _timer.Start();

                var soldiers = DataBaseHelper.Connection.Query<voin>(string.Format("Select * From voin where Id!={0}", Settings.mainSoldierId));
                if (soldiers.Count > 0)
                    Friends = new ObservableCollection<voin>(soldiers.OrderByDescending(soldier => soldier.Progress));

                Events = new ObservableCollection<importantDate>(DataBaseHelper.Connection.Query<importantDate>(
                         string.Format("select * from importantDate where VoinId={0} order by Date", Settings.mainSoldierId)));
                Events_CollectionChanged(null, null);
                Events.CollectionChanged += Events_CollectionChanged;

                BackgroundImage = GetBackground();
            }
            catch
            {
                App.Current.Exit();
            }

            if (Settings.notificationCountSetting > 0)
            {
                SelectedPage = 3;
                Settings.notificationCountSetting = 0;
            }
        }

        /// <summary>
        /// Возвращает фоновое изображение
        /// </summary>
        /// <returns></returns>
        private string GetBackground()
        {
            if (MainSoldier?.Progress >= 100)
                return "ms-appx:///Images/dmb.jpg";
            else
                switch (new AppSettings().Theme)
                {
                    case 1:
                        return GetThemePath("army", 3);
                    case 2:
                        return GetThemePath("car", 5);
                    case 3:
                        return GetThemePath("sport", 4);
                    case 4:
                        return GetThemePath("nature", 4);
                    case 5:
                        return GetThemePath("animals", 4);
                    case 6:
                        return "ms-appdata:///local/CustomTheme";
                    default:
                        return null;
                }
        }

        /// <summary>
        /// Получить путь к картинкам темы
        /// </summary>
        /// <param name="name">Название темы</param>
        /// <param name="count">Количество картинок</param>
        /// <returns></returns>
        private string GetThemePath(string name, int count)
        {
            return $"ms-appx:///Images/ThemesBackground/{name}_{new Random().Next(count)}.jpg";
        }

        /// <summary>
        /// Обновляем время по таймеру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, object e)
        {
            if (MainSoldier.Progress >= 100)
            {
                var last = MainSoldier.endDate - MainSoldier.beginDate;
                DmbTime = string.Format("Отслужено дней: {0}\nОтслужено часов: {1}\nОтслужено минут: {2}\nОтслужено секунд: {3}\n",
                    last.TotalDays, last.TotalHours, last.TotalMinutes, last.TotalSeconds);
                OnPropertyChanged("DmbTime");
                BackgroundImage = "ms-appx:///Images/dmb.jpg";
            }
            else
            {
                TimeLast = TimeHelper.TimeFormat(MainSoldier.Last);
                DaysLast = TimeHelper.DaysSpan(MainSoldier.beginDate, DateTime.Now);
                TimeRemain = TimeHelper.TimeFormat(MainSoldier.Remain);
                DaysRemain = TimeHelper.DaysSpan(DateTime.Now, MainSoldier.endDate);
                OnPropertiesChanged("DaysLast", "TimeLast", "DaysRemain", "TimeRemain");
            }
            OnPropertyChanged("MainSoldier");
        }

        #region MainSoldier

        /// <summary>
        /// Изменить главного солдата
        /// </summary>
        /// <param name="parameter"></param>
        private void ChangeMainSoldier(object parameter = null)
        {
            App.NavigationService.Navigate(typeof(View.AddSoldierView), MainSoldier);
        }

        /// <summary>
        /// Поделиться скриншотом
        /// </summary>
        /// <param name="parameter"></param>
        private void Share(object parameter = null)
        {
            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Отправляем скриншот в контакт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            try
            {
                args.Request.Data.Properties.Title = "#Дембель #WindowsPhone";
                args.Request.Data.SetText(MainSoldier.Progress >= 100 ? "ДЕМБЕЛЬ" : $"До дома {MainSoldier.Days}");

                var bitmap = new RenderTargetBitmap();
                // Скрываем статус бар, чтобы сделать скриншот
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
                await Task.Delay(10);
                await bitmap.RenderAsync(_screenShotContainer);
                await statusBar.ShowAsync();

                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("dmb.jpeg", CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    IBuffer pixelBuffer = await bitmap.GetPixelsAsync();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Ignore,
                                         (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight,
                                         96, 96, pixelBuffer.ToArray());

                    await encoder.FlushAsync();
                }
                args.Request.Data.SetStorageItems(new List<StorageFile> { file });
            }
            catch { }
            deferral.Complete();
        }

        #endregion

        #region Friends

        /// <summary>
        /// Добавить друга
        /// </summary>
        /// <param name="parameter"></param>
        private void AddFriend(object parameter = null)
        {
            _timer.Stop();
            App.NavigationService.Navigate(typeof(View.AddSoldierView));
        }

        /// <summary>
        /// Изменить друга
        /// </summary>
        /// <param name="parameter"></param>
        private void ChangeFriend(object parameter = null)
        {
            if (SelectedFriend != null)
                App.NavigationService.Navigate(typeof(View.AddSoldierView), SelectedFriend);
        }

        /// <summary>
        /// Удалить друга
        /// </summary>
        /// <param name="parameter"></param>
        private void DeleteFriend(object parameter = null)
        {
            if (SelectedFriend != null)
            {
                DataBaseHelper.Connection.Delete(SelectedFriend);
                Friends.Remove(SelectedFriend);
            }
        }

        /// <summary>
        /// Переместить на главную страницу
        /// </summary>
        /// <param name="parameter"></param>
        private void MakeMain(object parameter = null)
        {
            Settings.mainSoldierId = SelectedFriend.Id;
            App.NavigationService.Navigate(typeof(View.MainView));
            App.NavigationService.ClearBackStack();
        }

        #endregion

        #region Calendar

        /// <summary>
        /// Добавить событие
        /// </summary>
        /// <param name="parameter"></param>
        public async void AddEvent(object parameter = null)
        {

            if (!Settings.IsFullVersion)
            {
                await new MessageDialog("Добавлять события можно только в полной версии").ShowAsync();
                return;
            }

            if (_currentPopup != null)
                _currentPopup.IsOpen = false;

            var tbName = new TextBox();
            var tbMessage = new TextBlock { Text = "Название события" };
            var spanel = new StackPanel();
            spanel.Children.Add(tbMessage);
            spanel.Children.Add(tbName);

            ContentDialog eventNameDialog = new ContentDialog()
            {
                Content = spanel,
                Title = SelectedDate.ToString("d MMMM yyyy"),
                PrimaryButtonText = "Сохранить",
                SecondaryButtonText = "Отменить"
            };

            eventNameDialog.PrimaryButtonClick += (a, args) =>
            {
                if (tbName.Text == "")
                {
                    args.Cancel = true;
                    tbMessage.Text = "Недопустимое название";
                }
                else
                {
                    var newEvent = new importantDate()
                    {
                        VoinId = Settings.mainSoldierId,
                        Date = SelectedDate,
                        Name = tbName.Text
                    };
                    DataBaseHelper.Connection.Insert(newEvent);
                    // Находим место вставки нового события
                    var index = Events.IndexOf(Events.FirstOrDefault(e => e.Date > newEvent.Date));
                    Events.Insert(index >= 0 ? index : Events.Count, newEvent);
                }
            };
            await eventNameDialog.ShowAsync();
        }

        /// <summary>
        /// Показать уведомление календаря
        /// </summary>
        /// <param name="message">Текст уведомления</param>
        public async void ShowCalendarMessage()
        {
            StringBuilder sb = new StringBuilder();

            var events = DataBaseHelper.Connection.Query<importantDate>("select * from importantDate where VoinId=?", Settings.mainSoldierId).Where(e => e.Date == SelectedDate);
            foreach (var e in events)
                sb.AppendLine(e.Name);

            if (SelectedDate == MainSoldier.beginDate)
                sb.AppendLine("Призыв");

            if (SelectedDate == MainSoldier.endDate)
                sb.AppendLine("Демобилизация");

            if (events.Count(e => e.IsSystem) == 0 && SelectedDate >= MainSoldier.beginDate && SelectedDate < MainSoldier.endDate)
                sb.AppendLine($"{(SelectedDate - MainSoldier.beginDate).Days + 1} день службы");

            if (string.IsNullOrEmpty(sb.ToString().Trim()))
                return;

            if (_currentPopup != null)
                _currentPopup.IsOpen = false;
            var cPopup = new Controls.CalendarPopup(SelectedDate.ToString("d MMMM yyyy"), sb.ToString());
            var popup = new Popup { Child = cPopup };
            popup.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection();
            popup.ChildTransitions.Add(new Windows.UI.Xaml.Media.Animation.ContentThemeTransition());

            _currentPopup = popup;
            popup.IsOpen = true;
            await Task.Delay(3000);
            popup.IsOpen = false;
        }

        #endregion

        #region Events

        /// <summary>
        /// При изменении коллекции события обновляем календарь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Events_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(1);
            NearestEvent = Events.Where(d => !d.isGone).FirstOrDefault() ?? Events.LastOrDefault();
            SelectedEvent = NearestEvent;
            OnPropertyChanged("NearestEvent");
            _calendar.Refresh();
        }

        /// <summary>
        /// Удалить событие
        /// </summary>
        /// <param name="parameter"></param>
        private void DeleteEvent(object parameter = null)
        {
            if (SelectedEvent != null)
            {
                DataBaseHelper.Connection.Delete(SelectedEvent);
                Events.Remove(SelectedEvent);
            }
        }

        #endregion
    }
}
