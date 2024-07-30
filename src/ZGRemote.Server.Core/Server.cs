using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;
using ZGRemote.Server.Core.Handler;

namespace ZGRemote.Server.Core
{
    public class Server
    {
        public byte[] RsaBlobKey { get; set; }

        public int BuffSize { get; set; }

        public int MaxClient { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public bool IsRunning { get; set; }

        public List<User> UserList { get; private set; }

        private ZGServer server;

        public event Action<User> Connect;
        public event Action<User> DisConnect;

        public Server()
        {
            BuffSize = 512;
            MaxClient = 1024;
            IP = "127.0.0.1";
            Port = 9527;
            IsRunning = false;
        }

        public void Start()
        {
            server = new ZGServer(RsaBlobKey, BuffSize, MaxClient);
            UserList = new List<User>();
            server.Connect += OnConnect;
            server.DisConnect += OnDisconnect;
            server.Receive += OnReceive;
            server.Start(IP, Port);
            IsRunning = true;
        }

        public void Stop()
        {
            server.Connect -= OnConnect;
            server.DisConnect -= OnDisconnect;
            server.Receive -= OnReceive;
            server.Stop();
            server = null;
            IsRunning = false;
        }

        private void OnConnect(UserContext userContext)
        {
            Task.Run(() =>
            {
                try
                {
                    var info = SystemInfoHandler.GetSystemInfo(userContext);
                    if (info == null)
                    {
                        server.CloseClient(userContext);
                        return;
                    }

                    User user = new User()
                    {
                        IP = userContext.IP,
                        Port = userContext.Port,
                        Name = info["UserName"],
                        OperatingSystem = info["ComputerVersion"],
                        ComputerName = info["ComputerName"],
                        UserContext = userContext
                    };
                    lock (UserList) { UserList.Add(user); }
                    Connect?.Invoke(user);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "connect error");
                }
            });
        }

        private void OnDisconnect(UserContext userContext)
        {
            User user = UserList.FirstOrDefault(u => u.UserContext == userContext);
            if (user != null)
            {
                lock (UserList) { UserList.Remove(user); }
                DisConnect?.Invoke(user);
            }
        }

        private void OnReceive(UserContext userContext, byte[] data)
        {
            MessageBase message = MessageProcessor.UnPack(data);
            MessageProcessor.Process(userContext, message);
        }
    }
}