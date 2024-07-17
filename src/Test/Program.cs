using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZGRemote.Common;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;
using static System.Net.Mime.MediaTypeNames;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ZGClient client = new ZGClient(Settings.RSACSPBLOB);
            client.OnConnect += OnConnect;
            client.OnReceive += OnReceive;
            int i = 0;
            while (!client.Connect() && i <= 60)
            {
                i++;
                Thread.Sleep(1000);
            }
            if (client.Connected == false)
            {
                return;
            }
            while (true) { Thread.Sleep(1000); }
        }

        static void OnConnect(UserContext user)
        {
            DelegateHandlerProcessor.CreateAllDelegateHandlerInstanceByUserContext(user);
        }

        static void OnReceive(UserContext user, byte[] data)
        {
            MessageBase message = MessageProcessor.UnPack(data);
            MessageProcessor.Process(user, message);
        }
    }


}
