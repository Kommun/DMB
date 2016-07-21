using System;
using Windows.ApplicationModel.Background;
using Shared.Utils;
using Shared.Model;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace BackgroundTask
{
    public sealed class NotificationBackgroundTask : IBackgroundTask
    {
        AppSettings settings = new AppSettings();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                if (settings.IsRememberOn && settings.LastRemember != DateTime.Today.ToString() && DateTime.Now.Hour >= 7)
                {
                    var dates = DataBaseHelper.Connection.Query<importantDate>(string.Format("select * from importantDate where VoinId={0}", settings.mainSoldierId));
                    foreach (importantDate evnt in dates)
                        if (evnt.RemainDays == 0)
                        {
                            ShowToast(evnt.Name);
                            settings.notificationCountSetting++;
                        }
                    settings.LastRemember = DateTime.Today.ToString();
                }
                await TileHelper.Update();
            }
            catch { }
            deferral.Complete();
        }

        /// <summary>
        /// Показать уведомление
        /// </summary>
        /// <param name="message"></param>
        public static void ShowToast(string message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Дембель"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
