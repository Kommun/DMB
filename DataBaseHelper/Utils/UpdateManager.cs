using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Shared.Utils
{
    public static class UpdateManager
    {
        public const int CurrentVersion = 1;
        private static AppSettings _settings = new AppSettings();

        /// <summary>
        /// Отобразить изменения в текущем обновлении
        /// </summary>
        public static async Task ShowUpdateMessage()
        {
            if (_settings.AppVersion < CurrentVersion)
            {
                _settings.AppVersion = CurrentVersion;
                switch (CurrentVersion)
                {
                    case 1:
                        await new MessageDialog("v 3.3.1\n- Кнопки \"Документы\" и \"Поделиться\" с главного экрана вынесены в расширенное меню(вызывается нажатием на троеточие)." +
                            "\n- Поделиться теперь можно изображением с любой страницы главного экрана.\n- Исправлена ошибка, из - за которой пропали кнопки с нижнего бара.").ShowAsync();
                        break;
                }
            }
        }
    }
}
