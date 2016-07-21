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
using Shared.Utils;

namespace DMB.View
{
    public sealed partial class AddSoldierView : Page
    {
        public AddSoldierView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = new ViewModel.AddSoldierViewModel(e.Parameter as Shared.Model.voin);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (new AppSettings().mainSoldierId == 0)
                Application.Current.Exit();
        }
    }
}
