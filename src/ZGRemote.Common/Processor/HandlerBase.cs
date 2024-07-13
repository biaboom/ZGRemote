using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;

namespace ZGRemote.Common.Processor
{
    public abstract class HandlerBase
    {
        private static ConcurrentDictionary<Guid, MessageWaitEvent> messageWaitEventTable
            = new ConcurrentDictionary<Guid, MessageWaitEvent>();

        public static void Excute(UserContext user, MessageBase message)
        {
            if (messageWaitEventTable.TryRemove(message.ID, out MessageWaitEvent messageWaitEvent))
            {
                messageWaitEvent.Message = message;
            }
        }

        protected static MessageBase SendMessage(UserContext user, MessageBase message)
        {
            using (MessageWaitEvent messageWaitEvent = new MessageWaitEvent())
            {
                Guid guid = Guid.NewGuid();
                message.ID = guid;
                messageWaitEventTable.TryAdd(guid, messageWaitEvent);
                user.SendPack(MessageProcessor.Pack(message));

                messageWaitEvent.WaitMessage();

                messageWaitEventTable.TryRemove(guid, out _);
                return messageWaitEvent.Message;
                
            }

        }
    }
}
