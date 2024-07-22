using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZGRemote.Common.Message
{
    [ProtoContract]
    public class SystemInfoResponse : MessageBase
    {
        [ProtoMember(1)]
        public string ComputerName { get; set; }
        [ProtoMember(2)]
        public string ComputerVersion { get; set; }
        [ProtoMember(3)]
        public string UserName { get; set; }
    }
}
