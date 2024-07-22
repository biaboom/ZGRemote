using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Client.Utils;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Client.Handle
{
    [CanProcessMessage(typeof(SystemInfoRequest))]
    public class SystemInfoHandler : HandlerBase
    {
        public static new void Excute(UserContext user, MessageBase message)
        {
            switch (message)
            {
                case SystemInfoRequest systemInfoRequest:
                    GetSystemInfo(user, systemInfoRequest);
                    break;
            }
        }

        private static void GetSystemInfo(UserContext user, SystemInfoRequest message) 
        {
            SystemInfoResponse response = new SystemInfoResponse();
            response.ID = message.ID;
            response.ComputerName = SystemInfoUtil.MachineName;
            response.ComputerVersion = SystemInfoUtil.OsCaption;
            response.UserName = SystemInfoUtil.UserName;
            SendMessage(user, response);
        }
    }
}
