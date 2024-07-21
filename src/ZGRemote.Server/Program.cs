using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZGRemote.Common;
using ZGRemote.Common.Logging;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

using ZGRemote.Server.Handler;

namespace ZGRemote.Server
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }

        static void Init()
        {
            Logger.Init();

            ZGServer server = new ZGServer(Settings.RSACSPBLOB, 512, 1024);
            server.Connect += OnConnect;
            server.Receive += OnReceive;
            server.DisConnect += OnDisConnect;
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
                for(int i = 0; i < 20; i++)
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
                HandlerProcessor.ReleaseAllDelegateHandlerInstanceByUserContext(user);
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
