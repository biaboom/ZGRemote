using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZGRemote.Common.Message
{
    [ProtoContract]
    public class EchoRequest : MessageBase
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }
}
