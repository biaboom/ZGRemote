using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZGRemote.Common;
using ZGRemote.Common.Logging;
using ZGRemote.Common.Message;
using ZGRemote.Common.Message.cs;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;
using ZGRemote.Server.Handle;

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
            Application.Run(new Form1());
        }

        static void Init()
        {
            Logger.Init();

            ServerAsync server = new ServerAsync(Settings.RSACSPBLOB, 512, 1024);
            server.OnConnect += OnConnect;
            server.OnReceive += OnReceive;
            server.OnDisConnect += OnDisConnect;
            server.Start();
            
        }

        public static void OnReceive(UserContext user, byte[] data)
        {
            IMessage message = ProcessMessage.UnPack(data);
            ProcessMessage.Process(user, message);
        }

        public static void OnConnect(UserContext user)
        {
            SystemInfoHandle handle = SystemInfoHandle.CreateInstance(user);
            handle.GetSystemInfo(SystemInfoCallBack);
        }

        public static void OnDisConnect(UserContext user)
        {
            try
            {
                // 断开连接时释放所有handle
                ProcessHandle.ReleaseAllHandleInstanceByUserContext(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public static void SystemInfoCallBack(UserContext user, SystemInfoResponse message)
        {
            Log.Information(message.ComputerName);
            SystemInfoHandle.ReleaseInstance(user);
        }
    }
}
