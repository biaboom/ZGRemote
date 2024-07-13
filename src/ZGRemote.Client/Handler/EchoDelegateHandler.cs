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
    [CanProcessMessage(typeof(EchoRequest))]
    public class EchoDelegateHandler : DelegateHandlerBase<EchoDelegateHandler>
    {
        public static new void Excute(UserContext user, MessageBase message)
        {
            if (TryGetInstance(user, out EchoDelegateHandler instance))
            {
                switch (message)
                {
                    case EchoRequest echoRequest:
                        instance.Echo(user, echoRequest);
                    break;
                }
            }
        }

        private void Echo(UserContext user, EchoRequest message)
        {
            EchoResponse response = new EchoResponse();
            response.Message = message.Message;
            response.ID = message.ID;
            user.SendPack(MessageProcessor.Pack(response));
        }
    }
}
