using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;

namespace Shared.Utils
{
    public static class DataBaseHelper
    {
        private static string _dbPath = System.IO.Path.Combine(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "dbDMB.sqlite"));
        private static SQLiteConnection _connection;

        /// <summary>
        /// Соединение
        /// </summary>
        public static SQLiteConnection Connection
        {
            get
            {
                return _connection ?? (_connection = new SQLiteConnection(_dbPath));
            }
        }
    }
}
