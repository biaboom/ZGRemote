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

        protected static T SendMessage<T>(UserContext user, MessageBase message) where T : MessageBase
        {
            using (MessageWaitEvent messageWaitEvent = new MessageWaitEvent())
            {
                Guid guid = Guid.NewGuid();
                message.ID = guid;
                messageWaitEventTable.TryAdd(guid, messageWaitEvent);
                user.SendPack(MessageProcessor.Pack(message));

                T result = null;
                if(messageWaitEvent.WaitMessage())
                {
                    result =  messageWaitEvent.Message as T;
                }
                messageWaitEventTable.TryRemove(guid, out _);
                return result;
            }
        }

        protected static void SendMessage(UserContext user, MessageBase message)
        {
            user.SendPack(MessageProcessor.Pack(message));
        }
    }
}
