using Serilog;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;
using ZGRemote.Server.Handler;
using ZGRemote.Common.Logging;

namespace ZGRemote.Server.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

        }
        static void Init()
        {
            Logger.Init();

            ZGServer server = new ZGServer(Settings.RSACSPBLOB, 512, 1024);
            server.OnConnect += OnConnect;
            server.OnReceive += OnReceive;
            server.OnDisConnect += OnDisConnect;
            server.Start();

        }

        public static void OnReceive(UserContext user, byte[] data)
        {
            MessageBase message = MessageProcessor.UnPack(data);
            MessageProcessor.Process(user, message);
        }

        public static void OnConnect(UserContext user)
        {
            SystemInfoDelegateHandler handle = SystemInfoDelegateHandler.CreateInstance(user);
            handle.GetSystemInfoResponse += SystemInfoCallBack;
            handle.GetSystemInfo();
            Task.Run(() =>
            {
                EchoHandler.EchoMessage(user, "123");
                Stopwatch stopwatch = new Stopwatch();
                for (int i = 0; i < 20; i++)
                {
                    stopwatch.Restart();
                    EchoHandler.EchoMessage(user, "123");
                    stopwatch.Stop();
                    Log.Information(stopwatch.ElapsedTicks.ToString());


                }


            });
        }

        public static void OnDisConnect(UserContext user)
        {
            try
            {
                // 断开连接时释放所有handle
                DelegateHandlerProcessor.ReleaseAllDelegateHandlerInstanceByUserContext(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public static void SystemInfoCallBack(UserContext user, SystemInfoResponse message)
        {
            Log.Information(message.ComputerName);
        }
    }

}
