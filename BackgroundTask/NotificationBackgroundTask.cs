using System;
using Windows.ApplicationModel.Background;
using Shared.Utils;
using Shared.Model;

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
                            ToastHelper.ShowToast(evnt.Name);
                            settings.notificationCountSetting++;
                        }
                    settings.LastRemember = DateTime.Today.ToString();
                }
                await TileHelper.Update();
            }
            catch { }
            deferral.Complete();
        }
    }
}
