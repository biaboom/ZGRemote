using System;
using System.Collections.Generic;
using System.Text;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Server.Core.Handler
{
    public class SystemInfoHandler : HandlerBase
    {
        public static Dictionary<string, string> GetSystemInfo(UserContext user)
        {
            SystemInfoResponse response = SendMessage<SystemInfoResponse>(user, new SystemInfoRequest());
            if (response != null)
            {
                Dictionary<string, string> systemInfo = new Dictionary<string, string>();
                systemInfo.Add("UserName", response.UserName);
                systemInfo.Add("ComputerName", response.ComputerName);
                systemInfo.Add("ComputerVersion", response.ComputerVersion);
                return systemInfo;
            }
            return null;
        }
    }
}
