using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Server.Handler
{
    [CanProcessMessage(typeof(SystemInfoResponse))]
    internal class SystemInfoDelegateHandler : DelegateHandlerBase<SystemInfoDelegateHandler>
    {
        public event Action<UserContext, SystemInfoResponse> GetSystemInfoResponse;
        public static new void Excute(UserContext user, MessageBase message)
        {
            if(TryGetInstance(user, out SystemInfoDelegateHandler instance))
            {
                switch(message)
                {
                    case SystemInfoResponse systemInfoResponse:
                        instance.GetSystemInfoResponse?.Invoke(user, systemInfoResponse);
                    break;
                }
            }
        }

        public void GetSystemInfo()
        {
            SystemInfoRequest systemInfoRequest = new SystemInfoRequest();
            UserContext.SendPack(MessageProcessor.Pack(systemInfoRequest));
        }
    }
}
