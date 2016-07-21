using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Shared.Utils
{
    public class BackgroundTaskManager
    {
        public const string BackgroundTaskEntryPoint = "BackgroundTask.NotificationBackgroundTask";
        public const string BackgroundTaskName = "DMB.Notifications";

        public static async Task RegisterBackgroundTask()
        {
            if (!IsExistsTask())
            {
                await BackgroundExecutionManager.RequestAccessAsync();

                var builder = new BackgroundTaskBuilder();

                builder.Name = BackgroundTaskName;
                builder.TaskEntryPoint = BackgroundTaskEntryPoint;
                builder.SetTrigger(new TimeTrigger(30, false));

                builder.Register();
            }
        }

        public static bool IsExistsTask()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == BackgroundTaskName)
                    return true;
            return false;
        }
    }
}
