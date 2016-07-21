using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.UI.Popups;
using Windows.Storage;
using Shared.Utils;
using Shared.Model;

namespace DMB.ViewModel
{
    public class DocumentViewModel : PropertyChangedBase
    {
        /// <summary>
        /// Документ
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Текст документа
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="document"></param>
        public DocumentViewModel(object document)
        {
            Document = document as Document;
            ReadFile();
        }

        /// <summary>
        /// Прочитать текст из файла
        /// </summary>
        private async void ReadFile()
        {
            try
            {
                Text = await FileIO.ReadTextAsync(Document.File);
                OnPropertyChanged("Text");
            }
            catch
            {
                await new MessageDialog("Не удалось загрузить файл").ShowAsync();
                App.NavigationService.GoBack();
            }
        }
    }
}
