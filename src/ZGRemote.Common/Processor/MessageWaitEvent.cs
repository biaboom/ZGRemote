using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZGRemote.Common.Message;

namespace ZGRemote.Common.Processor
{
    public class MessageWaitEvent : IDisposable
    {
        private AutoResetEvent autoEvent;

        protected bool disposedValue;

        private MessageBase message;
        public MessageBase Message
        {
            get
            { return message; }
            set
            { 
                if(value != null)
                {
                    message = value;
                    autoEvent.Set();
                }
            }
        }

        public MessageWaitEvent() 
        {
            autoEvent = new AutoResetEvent(false);
        }

        public bool WaitMessage(int millisecondsTimeout = 15 * 1000)
        {
            return autoEvent.WaitOne(millisecondsTimeout);
        }

        public void Dispose() 
        { 
            if(disposedValue) return;
            autoEvent.Dispose();
            message = null;
            disposedValue = true;
            GC.SuppressFinalize(this);
        }
    }
}
