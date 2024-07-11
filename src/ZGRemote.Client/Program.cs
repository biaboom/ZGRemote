using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Application.Run(new Form1());
        }

        static void Init()
        {
#if DEBUG
            Logger.Init();
#endif

            Common.Networking.Client client = new Common.Networking.Client(Settings.RSACSPBLOB);
            client.OnConnect += OnConnect;
            client.OnReceive += OnReceive;
            int i = 0;
            while(!client.Connect() && i <= 60)
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
            ProcessHandle.CreateAllHandleInstanceByUserContext(user);
        }

        static void OnReceive(UserContext user, byte[] data)
        {
            try
            {
                IMessage message = ProcessMessage.UnPack(data);
                ProcessMessage.Process(user, message);
            }catch(Exception ex)
            {
                Log.Error("error", ex);
            }
        }
    }
}
