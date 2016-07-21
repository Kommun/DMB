using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel.Email;
using Windows.UI.Popups;
using Shared.Utils;

namespace DMB.ViewModel
{
    public class SettingsViewModel
    {

        public RelayCommand FullVersionCommand { get; set; }
        public RelayCommand FeedbackCommand { get; set; }
        public RelayCommand EmailCommand { get; set; }

        /// <summary>
        /// Настройки
        /// </summary>
        public AppSettings Settings { get; set; } = new AppSettings();

        /// <summary>
        /// Прозрачность блокиратора настроек
        /// </summary>
        public double LockOpacity
        {
            get { return Settings.IsFullVersion ? 1 : 0.2; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public SettingsViewModel()
        {
            FullVersionCommand = new RelayCommand(FullVersion);
            FeedbackCommand = new RelayCommand(Feedback);
            EmailCommand = new RelayCommand(Email);
        }

        /// <summary>
        /// Оставить отзыв
        /// </summary>
        /// <param name="parameter"></param>
        private async void Feedback(object parameter = null)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
            Settings.NotRated = false;
        }

        /// <summary>
        /// Купить полную версию
        /// </summary>
        /// <param name="parameter"></param>
        private async void FullVersion(object parameter = null)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["FullVersion"].IsActive)
            {
                try
                {
                    var result = await CurrentApp.RequestProductPurchaseAsync("FullVersion");
                    if (result.Status == ProductPurchaseStatus.Succeeded)
                        Settings.IsFullVersion = true;
                }
                catch { }
            }
            else Settings.IsFullVersion = true;
        }

        /// <summary>
        /// Отправить e-mail
        /// </summary>
        /// <param name="parameter"></param>
        private async void Email(object parameter = null)
        {
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Отзыв на приложение \"Дембель\"";
            mail.To.Add(new EmailRecipient("f11kostya@hotmail.com"));

            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private async void Lock()
        {
            await new MessageDialog("Данные настройки доступны только в полной версии").ShowAsync();
        }
    }
}
