using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Common.Message;
using ZGRemote.Common.Message.cs;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Processor;

namespace ZGRemote.Server.Handle
{
    [CanProcessMessage(typeof(SystemInfoResponse))]
    internal class SystemInfoHandle : HandleBase<SystemInfoHandle>
    {
        public event Action<UserContext, SystemInfoResponse> GetSystemInfoResponse;
        public static new void Excute(UserContext user, IMessage message)
        {
            if(TryGetInstance(user, out SystemInfoHandle instance))
            {
                switch(message)
                {
                    case SystemInfoResponse systemInfoResponse:
                        instance.GetSystemInfoResponse?.Invoke(user, systemInfoResponse);
                    break;
                }
            }
        }

        public void GetSystemInfo(Action<UserContext, SystemInfoResponse> CallBack = null)
        {
            if (CallBack != null) GetSystemInfoResponse += CallBack;
            SystemInfoRequest systemInfoRequest = new SystemInfoRequest();
            UserContext.SendPack(ProcessMessage.Pack(systemInfoRequest));
        }
    }
}
