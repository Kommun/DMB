using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utils
{
    public static class TimeHelper
    {
        /// <summary>
        /// Возвращает временной промежуток в заданном формате
        /// </summary>
        /// <param name="from">Начальная дата</param>
        /// <param name="to">Конечная дата</param>
        /// <returns></returns>
        public static string DaysSpan(DateTime from, DateTime to)
        {
            int days, weeks;
            DateTimeSpan dtSpan;

            switch (new AppSettings().DisplayTimeFormat)
            {
                case 0:
                    days = (to - from).Days;
                    return string.Format("{0} {1}", days, Days(days));
                case 1:
                    weeks = (to - from).Days / 7;
                    days = (to - from).Days % 7;
                    return string.Format("{0} {1}\n{2} {3}", weeks, Weeks(weeks), days, Days(days));
                case 2:
                    dtSpan = DateTimeSpan.CompareDates(from, to);
                    return string.Format("{0} {1}\n{2} {3}", dtSpan.Months, strMonths(dtSpan.Months), dtSpan.Days, Days(dtSpan.Days));
                case 3:
                    dtSpan = DateTimeSpan.CompareDates(from, to);
                    weeks = dtSpan.Days / 7;
                    days = dtSpan.Days % 7;
                    return string.Format("{0} {1}\n{2} {3}\n{4} {5}", dtSpan.Months, strMonths(dtSpan.Months), weeks, Weeks(weeks), days, Days(days));
                default:
                    return "";
            }
        }

        /// <summary>
        /// Возвращает время в заданном формате
        /// </summary>
        /// <param name="time">Время</param>
        /// <returns></returns>
        public static string TimeFormat(TimeSpan time)
        {
            return string.Format("{0} {1}\n{2} {3}\n{4} {5}", time.Hours, Hours(time.Hours), time.Minutes, Minutes(time.Minutes), time.Seconds, Seconds(time.Seconds));
        }

        public static string strMonths(int count)
        {
            int ost = count % 100;
            if ((ost >= 5 && ost <= 20) || ost == 0)
                return "месяцев";
            else
            {
                ost = count % 10;
                if (ost == 1)
                    return "месяц";
                else if (ost >= 2 && ost <= 4)
                    return "месяца";
                else return "месяцев";
            }
        }

        /// <summary>
        /// Недели
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Weeks(int count)
        {
            int ost = count % 100;
            if ((ost >= 5 && ost <= 20) || ost == 0)
                return "недель";
            else
            {
                ost = count % 10;
                if (ost == 1)
                    return "неделя";
                else if (ost >= 2 && ost <= 4)
                    return "недели";
                else return "недель";
            }
        }

        /// <summary>
        /// Дни
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Days(int count)
        {
            int ost = count % 100;
            if ((ost >= 5 && ost <= 20) || ost == 0)
                return "дней";
            else
            {
                ost = count % 10;
                if (ost == 1)
                    return "день";
                else if (ost >= 2 && ost <= 4)
                    return "дня";
                else return "дней";
            }
        }

        /// <summary>
        /// Часы
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Hours(int count)
        {
            if ((count >= 5 && count <= 20) || count == 0)
                return "часов";
            else
            {
                int ost = count % 10;
                if (ost == 1)
                    return "час";
                else if (ost >= 2 && ost <= 4)
                    return "часа";
                else return "часов";
            }
        }

        /// <summary>
        /// Минуты
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Minutes(int count)
        {
            if ((count >= 5 && count <= 20) || count == 0)
                return "минут";
            else
            {
                int ost = count % 10;
                if (ost == 1)
                    return "минута";
                else if (ost >= 2 && ost <= 4)
                    return "минуты";
                else return "минут";
            }
        }

        /// <summary>
        /// Секунды
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Seconds(int count)
        {
            if ((count >= 5 && count <= 20) || count == 0)
                return "секунд";
            else
            {
                int ost = count % 10;
                if (ost == 1)
                    return "секунда";
                else if (ost >= 2 && ost <= 4)
                    return "секунды";
                else return "секунд";
            }
        }

        /// <summary>
        /// Осталось
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Remain(int count)
        {
            if (count % 10 == 1 && count % 100 != 11)
                return "Остался";
            else
                return "Осталось";
        }

        /// <summary>
        /// Прошло
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        public static string Gone(int count)
        {
            if (count % 10 == 1 && count % 100 != 11)
                return "Прошел";
            else
                return "Прошло";
        }
    }
}
