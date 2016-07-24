using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace Shared.Utils
{
    public static class ToastHelper
    {
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
