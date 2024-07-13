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
    public class EchoHandler : HandlerBase
    {
        public static string EchoMessage(UserContext user, string message)
        {
            EchoResponse response = SendMessage(user, new EchoRequest() { Message = message }) as EchoResponse;
            return response?.Message;
        }
    }
}
