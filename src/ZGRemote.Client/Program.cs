using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZGRemote.Client.Handle;
using ZGRemote.Common.Logging;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Client
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Task.Run(() => Init());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }

        static void Init()
        {
#if DEBUG
            Logger.Init();
#endif

            ZGClient client = new ZGClient(Settings.RSACSPBLOB);
            client.Connect += OnConnect;
            client.Receive += OnReceive;
            int i = 0;
            while(!client.ConnectServer() && i <= 60)
            {
                i++;
                Thread.Sleep(1000);
            }
            if(client.Connected == false)
            {
                Application.Exit();
            }

        }

        static void OnConnect(UserContext user)
        {
            HandlerProcessor.CreateAllDelegateHandlerInstanceByUserContext(user);
        }

        static void OnReceive(UserContext user, byte[] data)
        {
            try
            {
                MessageBase message = MessageProcessor.UnPack(data);
                MessageProcessor.Process(user, message);
            }catch(Exception ex)
            {
                Log.Error("error", ex);
            }
        }
    }
}
