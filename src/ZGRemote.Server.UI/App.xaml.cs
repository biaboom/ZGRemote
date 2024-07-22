using System.Configuration;
using System.Data;
using System.Windows;
using ZGRemote.Server.UI.ViewModels;

namespace ZGRemote.Server.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainViewModel MainViewModel {  get; set; }

        public App()
        {
            MainViewModel = new MainViewModel();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = MainViewModel;
            mainWindow.Show();
        }
    }

}
