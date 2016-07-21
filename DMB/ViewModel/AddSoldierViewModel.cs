using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Shared.Utils;
using Shared.Model;

namespace DMB.ViewModel
{
    public class AddSoldierViewModel : PropertyChangedBase
    {
        private voin _soldier;
        private AppSettings _settings = new AppSettings();
        private DateTimeOffset _beginDate;

        public RelayCommand SaveCommand { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Дата начала службы
        /// </summary>
        public DateTimeOffset BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                EndDate = _beginDate.AddYears(1);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата окончания службы
        /// </summary>
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="soldier"></param>
        public AddSoldierViewModel(voin soldier)
        {
            SaveCommand = new RelayCommand(Save);
            if (soldier != null)
            {
                _soldier = soldier;
                Name = _soldier.name;
                _beginDate = _soldier.beginDate;
                EndDate = _soldier.endDate;
            }
            else
                BeginDate = DateTime.Now;
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        /// <param name="parameter"></param>
        private async void Save(object parameter = null)
        {
            if (Name.ToLower() == "ihavealreadybought")
            {
                new AppSettings().IsFullVersion = true;
                App.NavigationService.GoBack();
            }
            else if (Name == "")
                await new MessageDialog("Введите имя").ShowAsync();
            else if (EndDate.Date.Date <= BeginDate.Date.Date)
                await new MessageDialog("Дата окончания службы должна быть больше даты начала").ShowAsync();
            else
            {
                var newSoldier = new voin()
                {
                    name = Name,
                    beginDate = BeginDate.Date.Date,
                    endDate = EndDate.Date.Date
                };

                if (_soldier != null)
                {
                    newSoldier.Id = _soldier.Id;
                    DataBaseHelper.Connection.Update(newSoldier);
                }
                else
                    DataBaseHelper.Connection.Insert(newSoldier);

                if (_settings.mainSoldierId == 0)
                    _settings.mainSoldierId = newSoldier.Id;

                DataBaseHelper.Connection.Execute(string.Format("delete from importantDate where IsSystem AND VoinId={0}", newSoldier.Id));

                var daysCount = (newSoldier.endDate - newSoldier.beginDate).Days;
                for (int i = 1; i <= daysCount / 100; i++)
                {
                    DataBaseHelper.Connection.Insert(new importantDate()
                    {
                        VoinId = newSoldier.Id,
                        Name = $"{i * 100} дней до дембеля",
                        Date = newSoldier.endDate.AddDays(-i * 100),
                        IsSystem = true
                    });
                    DataBaseHelper.Connection.Insert(new importantDate()
                    {
                        VoinId = newSoldier.Id,
                        Name = $"{i * 100} дней после призыва",
                        Date = newSoldier.beginDate.AddDays(i * 100),
                        IsSystem = true
                    });
                }

                DataBaseHelper.Connection.Insert(new importantDate() { VoinId = newSoldier.Id, Name = "Половина службы", Date = newSoldier.beginDate.AddDays((newSoldier.endDate - newSoldier.beginDate).Days / 2), IsSystem = true });

                App.NavigationService.GoBack();
            }
        }
    }
}
