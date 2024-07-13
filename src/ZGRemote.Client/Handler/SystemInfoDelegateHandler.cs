using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Client.Handle
{
    [CanProcessMessage(typeof(SystemInfoRequest))]
    public class SystemInfoDelegateHandler : DelegateHandlerBase<SystemInfoDelegateHandler>
    {
        public static new void Excute(UserContext user, MessageBase message)
        {
            if (TryGetInstance(user, out SystemInfoDelegateHandler instance))
            {
                switch (message)
                {
                    case SystemInfoRequest systemInfoRequest:
                        instance.GetSystemInfo(user, systemInfoRequest);
                    break;
                }
            }
        }

        public void GetSystemInfo(UserContext user, SystemInfoRequest message) 
        {
            SystemInfoResponse response = new SystemInfoResponse();
            response.ID = message.ID;
            response.ComputerName = "Test";
            user.SendPack(MessageProcessor.Pack(response));
        }
    }
}
