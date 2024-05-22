using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZGRemote.Common.Message.cs
{
    [ProtoContract]
    public class SystemInfoResponse : IMessage
    {
        [ProtoMember(1)]
        public string ComputerName { get; set; }
        [ProtoMember(2)]
        public string ComputerVersion { get; set; }
        [ProtoMember(3)]
        public string CpuName { get; set; }
        [ProtoMember(4)]
        public string PhysicalMemory { get; set; } //MB
        [ProtoMember(5)]
        public string DiskSize { get; set; } //MB
        [ProtoMember(6)]
        public string[] MAC { get; set; }
    }
}
