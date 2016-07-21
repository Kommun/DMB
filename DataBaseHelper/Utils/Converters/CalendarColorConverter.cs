using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Shared.Model;
using Shared.Utils;

namespace Shared.Utils.Converters
{
    public class CalendarColorConverter : DependencyObject, IValueConverter
    {
        #region Properties

        #region CurrentSoldierProperty
        /// <summary>
        /// Главный солдат
        /// </summary>
        public voin CurrentSoldier
        {
            get { return (voin)GetValue(CurrentSoldierProperty); }
            set { SetValue(CurrentSoldierProperty, value); }
        }

        public static readonly DependencyProperty CurrentSoldierProperty =
            DependencyProperty.Register("CurrentSoldier", typeof(voin), typeof(CalendarColorConverter), new PropertyMetadata(null));
        #endregion

        #region EventsProperty
        /// <summary>
        /// События главного солдата
        /// </summary>
        public ObservableCollection<importantDate> Events
        {
            get { return (ObservableCollection<importantDate>)GetValue(EventsProperty); }
            set { SetValue(EventsProperty, value); }
        }

        public static readonly DependencyProperty EventsProperty =
            DependencyProperty.Register("Events", typeof(ObservableCollection<importantDate>), typeof(CalendarColorConverter), new PropertyMetadata(null));
        #endregion

        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Последний отслуженный день
            var _coloredDate = CurrentSoldier.endDate > DateTime.Today ? DateTime.Today : CurrentSoldier.endDate;

            if ((DateTime)value == CurrentSoldier.beginDate || (DateTime)value == CurrentSoldier.endDate || Events.Count(d => d.Date == (DateTime)value && !d.isGone) > 0)
                return new SolidColorBrush(Colors.Red);
            else if (Events.Count(d => d.Date == (DateTime)value && d.isGone) > 0)
                return new SolidColorBrush(Colors.Gray);
            else if ((DateTime)value >= CurrentSoldier.beginDate && (DateTime)value <= _coloredDate)
                return new SolidColorBrush(Colors.Green);
            else
                return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
