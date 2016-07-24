using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Shared.Utils;

namespace Shared.Model
{
    public class importantDate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Id солдата
        /// </summary>
        public int VoinId { get; set; }

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Системное событие
        /// </summary>
        public bool IsSystem { get; set; }

        [Ignore]
        /// <summary>
        /// Строкове представление даты события
        /// </summary>
        public string DateString
        {
            get { return Date.ToString("d MMMM yyyy"); }
        }

        [Ignore]
        /// <summary>
        /// Количество дней до/после события
        /// </summary>
        public string Remain
        {
            get
            {
                var daysCount = Math.Abs(RemainDays);

                if (RemainDays == 0)
                    return "Сегодня";
                else
                    return string.Format("{0} {1} {2}", RemainDays > 0 ? TimeHelper.Remain(RemainDays) :
                        TimeHelper.Gone(daysCount), daysCount, TimeHelper.Days(daysCount));
            }
        }

        [Ignore]
        /// <summary>
        /// Осталось дней
        /// </summary>
        public int RemainDays
        {
            get { return (Date - DateTime.Today).Days; }
        }

        [Ignore]
        /// <summary>
        /// Прошло
        /// </summary>
        public bool isGone
        {
            get { return RemainDays < 0; }
        }

        [Ignore]
        /// <summary>
        /// Прозрачность
        /// </summary>
        public double Opacity
        {
            get { return isGone ? 0.6 : 1; }
        }

        [Ignore]
        /// <summary>
        /// Путь к картинке
        /// </summary>
        public string ImgSource
        {
            get
            {
                if (RemainDays > 0)
                    return "/Images/calendarOrange.png";
                else if (RemainDays == 0)
                    return "/Images/calendarGreen.png";
                else
                    return "/Images/calendarGray.png";
            }
        }
    }
}
