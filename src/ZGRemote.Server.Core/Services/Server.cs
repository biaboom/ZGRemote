using System;
using System.Collections.Generic;
using System.Text;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;
using ZGRemote.Server.Core.Models;

namespace ZGRemote.Server.Core.Services
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

        private ZGServer zgserver;

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
            zgserver = new ZGServer(RsaBlobKey, BuffSize, MaxClient);
            zgserver.Connect += OnConnect;
            zgserver.DisConnect += OnDisconnect;
            zgserver.Receive += OnReceive;
            zgserver.Start(IP, Port);
        }

        public void Stop() 
        { 
            zgserver.Connect -= OnConnect;
            zgserver.DisConnect -= OnDisconnect;
            zgserver.Receive -= OnReceive;
            zgserver.Stop();
            zgserver = null;
        }

        private void OnConnect(UserContext userContext)
        {
            
        }

        private void OnDisconnect(UserContext userContext)
        {

        }

        private void OnReceive(UserContext userContext, byte[] data)
        {
            MessageProcessor.Process(userContext, MessageProcessor.UnPack(data));
        }
    }
}
