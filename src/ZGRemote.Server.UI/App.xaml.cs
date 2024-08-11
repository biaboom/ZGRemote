using System.Configuration;
using System.Data;
using System.Windows;
using ZGRemote.Server.UI.ViewModels;
using ZGRemote.Server.Core;
using Serilog.Events;
using Serilog;
using ZGRemote.Common.Logging;

namespace ZGRemote.Server.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainViewModel MainViewModel { get; private set; }

        public static new App Current => (App)Application.Current;

        public Core.Server Server { get; set; }

        public App()
        {
            Logger.Init();
            Server = new Core.Server();
            Server.RsaBlobKey = Settings.RSACSPBLOB;
            MainViewModel = new MainViewModel();
            

#if DEBUG
            Server.Start();
#endif
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow(MainViewModel);
            mainWindow.Show();

        }
    }

}
