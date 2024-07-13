using ProtoBuf;
using System;

namespace ZGRemote.Common.Message
{
    [ProtoContract]
    public abstract class MessageBase
    {
        [ProtoMember(64)]
        public Guid ID { get; set; }
    }
}