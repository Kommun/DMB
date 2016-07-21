using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Shared.Model
{
    public class Document
    {
        /// <summary>
        /// Название документа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Файл
        /// </summary>
        public StorageFile File { get; set; }
    }
}
