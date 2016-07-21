using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Shared.Utils
{
    public class NavigationService
    {
        readonly Frame frame;

        public NavigationService(Frame frame)
        {
            this.frame = frame;
        }

        /// <summary>
        /// Переход назад
        /// </summary>
        public void GoBack()
        {
            frame.GoBack();
        }

        /// <summary>
        /// Переход вперед
        /// </summary>
        public void GoForward()
        {
            frame.GoForward();
        }

        /// <summary>
        /// Навигация
        /// </summary>
        /// <typeparam name="T">Тип страницы</typeparam>
        /// <param name="parameter">Параметр навигации</param>
        /// <returns></returns>
        public bool Navigate<T>(object parameter = null)
        {
            var type = typeof(T);

            return Navigate(type, parameter);
        }

        /// <summary>
        /// Навигация
        /// </summary>
        /// <param name="source">Тип страницы</param>
        /// <param name="parameter">Параметр навигации</param>
        /// <returns></returns>
        public bool Navigate(Type source, object parameter = null)
        {
            return frame.Navigate(source, parameter);
        }

        /// <summary>
        /// Очистить историю переходов
        /// </summary>
        public void ClearBackStack()
        {
            frame.BackStack.Clear();
        }
    }
}
