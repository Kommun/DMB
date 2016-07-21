using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Shared.Utils;

namespace Shared.Model
{
    public class voin
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Дата начала службы
        /// </summary>
        public DateTime beginDate { get; set; }

        /// <summary>
        /// Дата окончания службы
        /// </summary>
        public DateTime endDate { get; set; }

        [Ignore]
        /// <summary>
        /// Строковое представления даты окончания службы
        /// </summary>
        public string stringEndDate
        {
            get { return endDate.ToString("d MMM yyyy"); }
        }

        [Ignore]
        /// <summary>
        /// Интервал прошедшего времени
        /// </summary>
        public TimeSpan Last
        {
            get { return DateTime.Now - beginDate; }
        }

        [Ignore]
        /// <summary>
        /// Интервал оставшегося времени
        /// </summary>
        public TimeSpan Remain
        {
            get { return endDate - DateTime.Now; }
        }

        [Ignore]
        /// <summary>
        /// Осталось дней
        /// </summary>
        public string Days
        {
            get
            {
                if (Progress >= 100)
                    return "Дембель";
                else
                    return $"{Remain.Days} { TimeHelper.Days(Remain.Days)}";
            }
        }

        [Ignore]
        /// <summary>
        /// Оставшееся время на плитке
        /// </summary>
        public string TileTime
        {
            get
            {
                if (Progress >= 100)
                    return "Дембель";
                else if (Remain.Days > 0)
                    return $"{Remain.Days} { TimeHelper.Days(Remain.Days)}";
                else
                    return $"{Remain.Hours} { TimeHelper.Hours(Remain.Hours)}";
            }
        }

        [Ignore]
        /// <summary>
        /// Прогресс прохождения службы
        /// </summary>
        public double Progress
        {
            get
            {
                var progress = (double)Last.TotalSeconds * 100 / (endDate - beginDate).TotalSeconds;

                if (progress > 100)
                    progress = 100;
                if (progress < 0)
                    progress = 0;

                return progress;
            }

        }

        [Ignore]
        /// <summary>
        /// Процент прошедшей службы
        /// </summary>
        public string ProgressPercent
        {
            get
            {
                var settings = new AppSettings();
                var progress = Math.Round(Progress - Math.Pow(10, -settings.PercentDigitsNumber) / 2, settings.PercentDigitsNumber);
                return $"{progress}%";
            }
        }
    }
}
