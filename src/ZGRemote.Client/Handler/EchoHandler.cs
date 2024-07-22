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
    public class EchoHandler : HandlerBase
    {
        public static new void Excute(UserContext user, MessageBase message)
        {
            switch (message)
            {
                case EchoRequest echoRequest:
                    Echo(user, echoRequest);
                    break;
            }
        }

        private static void Echo(UserContext user, EchoRequest message)
        {
            EchoResponse response = new EchoResponse();
            response.Message = message.Message;
            response.ID = message.ID;
            SendMessage(user, response);
        }
    }
}
