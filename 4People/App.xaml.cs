using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Services;
using _4People.ViewModels;

namespace _4People
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainViewModel mainViewModel = new()
            {
                IsDatabaseLoading = true
            };

            MainWindow mainWindow = new(mainViewModel);
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = mainWindow;
            mainWindow.Show();

            StorageFacade.Instance.Init()
                         .ContinueWith(_ => mainViewModel.FillFields())
                         .ContinueWith(_ => Current.Dispatcher.Invoke(() =>
                         {
                             mainViewModel.IsDatabaseLoading = false;
                         }));
        }
    }
}
