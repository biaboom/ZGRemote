using System;
using System.Collections.Generic;
using System.Text;
using ZGRemote.Common.Networking;

namespace ZGRemote.Server.Core.Models
{
    public class User
    {
        public string Name { get; set; }

        public string IP {  get; set; }

        public int Port { get; set; }

        public string OperatingSystem { get; set; }

        public UserContext UserContext { get; set; }
    }
}
