using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.ApplicationModel;
using Shared.Utils;
using Shared.Model;

namespace DMB.ViewModel
{
    public class DocumentsListViewModel : PropertyChangedBase
    {
        //private Document _selectedDocument;

        /// <summary>
        /// Список документов
        /// </summary>
        public List<Document> Documents { get; set; }

        /// <summary>
        /// Выбранный документ
        /// </summary>
        public Document SelectedDocument
        {
            set { App.NavigationService.Navigate(typeof(View.DocumentView), value); }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DocumentsListViewModel()
        {
            RefreshDocuments();
        }

        /// <summary>
        /// Обновить список документов
        /// </summary>
        private async void RefreshDocuments()
        {
            try
            {
                var folder = await Package.Current.InstalledLocation.GetFolderAsync("Documents");
                var files = await folder.GetFilesAsync();

                if (files.Count > 0)
                {
                    Documents = files.Select(f => new Document { Name = f.DisplayName, File = f }).ToList();
                    OnPropertyChanged("Documents");
                    return;
                }
            }
            catch { }

            await new MessageDialog("Не удалось загрузить список документов").ShowAsync();
            App.NavigationService.GoBack();
        }
    }
}
