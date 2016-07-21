using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Store;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.ApplicationModel.Background;
using Shared.Utils;
using Shared.Model;

namespace DMB
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        private AppSettings _settings = new AppSettings();

        public static NavigationService NavigationService { get; set; }

        /// <summary>
        /// Инициализирует одноэлементный объект приложения.  Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// если приложение запускается для открытия конкретного файла, отображения
        /// результатов поиска и т. д.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await BackgroundTaskManager.RegisterBackgroundTask();

            int tileId;
            if (int.TryParse(e.TileId, out tileId))
                _settings.mainSoldierId = tileId;
            if (_settings.IsFirstLaunch)
            {
                DataBaseHelper.Connection.CreateTable<voin>();
                DataBaseHelper.Connection.CreateTable<importantDate>();

                _settings.AppVersion = UpdateManager.CurrentVersion;
                _settings.IsFirstLaunch = false;
            }

            _settings.Runs++;
            if (_settings.NotRated && _settings.Runs == 4)
            {
                MessageDialog msgbox = new MessageDialog("Вы пользуетесь приложением уже достаточно долго. Не могли бы Вы оставить о нем отзыв?");

                msgbox.Commands.Add(new UICommand("Да", async c =>
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
                    _settings.NotRated = false;
                }));
                msgbox.Commands.Add(new UICommand("Нет"));

                await msgbox.ShowAsync();
                _settings.Runs = 0;
            }

            await UpdateManager.ShowUpdateMessage();

            if (CurrentApp.LicenseInformation.ProductLicenses["FullVersion"].IsActive)
                _settings.IsFullVersion = true;

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();
                NavigationService = new NavigationService(rootFrame);

                // TODO: Измените это значение на размер кэша, подходящий для вашего приложения
                rootFrame.CacheSize = 1;

                // Задайте язык по умолчанию
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Загрузить состояние из ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Удаляет турникетную навигацию для запуска.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации
                if (!rootFrame.Navigate(typeof(View.MainView), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
                if (_settings.mainSoldierId == 0)
                    rootFrame.Navigate(typeof(View.AddSoldierView));
            }

            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }

        /// <summary>
        /// Восстанавливает переходы содержимого после запуска приложения.
        /// </summary>
        /// <param name="sender">Объект, где присоединен обработчик.</param>
        /// <param name="e">Сведения о событии перехода.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Сохранить состояние приложения и остановить все фоновые операции
            deferral.Complete();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            // Сохраняем выбранную пользователем картинку
            if (args is FileOpenPickerContinuationEventArgs)
                try
                {
                    var files = (args as FileOpenPickerContinuationEventArgs).Files;
                    if (files.Count > 0)
                        await files[0]?.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, "CustomTheme", Windows.Storage.NameCollisionOption.ReplaceExisting);
                    else
                        _settings.Theme = 0;
                }
                catch { }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
            else App.Current.Exit();
        }
    }
}