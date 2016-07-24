using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DMB.Controls
{
    public sealed partial class CalendarPopup : UserControl
    {
        /// <summary>
        /// Дата
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="date"></param>
        /// <param name="message"></param>
        public CalendarPopup(string date, string message)
        {
            this.InitializeComponent();
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;

            Date = date;
            Message = message;
            DataContext = this;
        }
    }
}
